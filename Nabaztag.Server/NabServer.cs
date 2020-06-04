using Nabaztag.Net.Models;
using Nabaztag.Server.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Device.Gpio;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;
using System.Xml.Linq;

namespace Nabaztag.Server
{
    public class NabServer
    {
        private const int PinButton = 17;
        private const string PyNabDirectory = "pynab";
        private const string SoundDirectory = "sounds";
        private const string ChoreographyDirectory = "choreographies";

        public static Leds Leds;
        public static Button Button;
        public static Sound Sound;
        public static Ears Ears;

        public static bool IsBusy => Sound.IsPlaying || Sound.IsRecording || Ears.IsAnyMoving || Choreography.IsChoreographyPlaying;

        private static string _path;

        private static TcpListener _tcpListener;
        private static NabState _state = new NabState() { State = StateType.Asleep };
        private static Dictionary<Socket, EventNotification> _eventId;
        private static byte[] _buffer = new byte[7000];
        private static int _connectedClients = 0;
        private static bool _applicationRunning = true;
        private static string _locale;
        private static Settings _settings;
        private static List<Thread> _listnerThread;
        private static object _threadListLock;

        private static List<Socket> _sockets;
        private static Queue<Process> _queueProcess;
        private static object _socketLock;
        private static object _animationLock;

        //private static Thread _animationIdle;
        private static Animation _animation = null;

        static void Main(string[] args)
        {
            _listnerThread = new List<Thread>();
            _path = Directory.GetCurrentDirectory();
            _eventId = new Dictionary<Socket, EventNotification>();
            _sockets = new List<Socket>();
            _queueProcess = new Queue<Process>();
            _socketLock = new object();
            _threadListLock = new object();
            _animationLock = new object();

            try
            {
                _settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText($"{_path}/settings.json"));
                _locale = _settings.Locale.Replace('-', '_');
            }
            catch
            { }

            Leds = new Leds();
            Button = new Button(PinButton);
            Button.ButtonEvent += Button_ButtonEvent;

            Sound = new Sound();
            Ears = new Ears();

            WakeUp();

            _tcpListener = new TcpListener(IPAddress.Any, 10543);
            _tcpListener.Start();

            byte[] buffer = new byte[7000];

            _state.State = StateType.Idle;
            Console.WriteLine("Press the button and watch the events, press a key to exit");
            Console.WriteLine("Press long to record and it will play after");
            Console.WriteLine("Double click to play a choreography");

            var listn = new Thread(() =>
            {
                while (_applicationRunning)
                {
                    Thread.Sleep(10);
                    ListenToSockets();
                }
            });
            listn.Priority = ThreadPriority.Lowest;
            listn.Start();

            while (!Console.KeyAvailable)
            {
                try
                {
                    Animation toPlay;
                    lock (_animationLock)
                    {
                        toPlay = _animation;
                    }
                    PlayAnimation(toPlay);
                    //Thread.Sleep(10);

                    var numToProcess = _queueProcess.Count();
                    for (int i = 0; i < numToProcess; i++)
                    {
                        var proc = _queueProcess.Peek();
                        var numListners = _sockets.Count();
                        for (int sk = 0; sk < numListners; sk++)
                        {
                            if (!_sockets[sk].Connected)
                            {
                                _sockets.Remove(_sockets[sk]);
                            }
                        }

                        switch (proc.PaquetType)
                        {
                            case PaquetType.Information:
                                ProcessInformation((Info)proc.ToProcess);
                                break;
                            case PaquetType.Ears:
                                ProcessMoveEar((Net.Models.Ears)proc.ToProcess);
                                break;
                            case PaquetType.Command:
                                ProcessCommand((Command)proc.ToProcess);
                                break;
                            case PaquetType.Message:
                                ProcessMessage((Message)proc.ToProcess);
                                break;
                            case PaquetType.Wakeup:
                                ProcessWakeUp();
                                break;
                            case PaquetType.Sleep:
                                ProcessGoToSleep();
                                break;
                            case PaquetType.Test:
                                ProcessTest((TestMode)proc.ToProcess);
                                break;
                            case PaquetType.Cancel:
                            case PaquetType.Mode:
                            case PaquetType.EarEvent:
                            case PaquetType.EarsEvent:
                            case PaquetType.ButtonEvent:
                            case PaquetType.Response:
                            case PaquetType.AsrEvent:
                            case PaquetType.Statistics:
                            case PaquetType.State:
                            default:
                                break;
                        }
                        _queueProcess.Dequeue();
                    }
                }
                catch (Exception ex)
                {
                    _state.State = StateType.Idle;
                    Console.WriteLine($"Exception in main loop: {ex}");
                }

            }

            _applicationRunning = false;
            _tcpListener.Stop();

            try
            {
                listn.Abort();
                //lock (_threadListLock)
                //{
                //    foreach (var th in _listnerThread)
                //    {
                //        if (th.IsAlive)
                //        {
                //            th.Abort();
                //        }
                //    }
                //}
            }
            catch
            { }

            Button.Dispose();

            GoToSleep();
        }

        private static void Button_ButtonEvent(object sender, ButtonEventArguments buttonEventArgs)
        {
            try
            {
                Console.WriteLine($"Event: {buttonEventArgs.ButtonEventType}, {buttonEventArgs.DateTimeEvent}");
                if ((buttonEventArgs.ButtonEventType == ButtonEventType.DoubleClick) && (!Choreography.IsChoreographyPlaying))
                {
                    var fileName = FindChoreography("*.chor");
                    Console.WriteLine($"Playing choreography ${fileName}");
                    Choreography.ReadChoreography(fileName);
                }
                else if (buttonEventArgs.ButtonEventType == ButtonEventType.Hold)
                {
                    // Console.WriteLine("Playing listening");
                    _state.State = StateType.Playing;
                    Sound.Play($"{_path}/sounds/listen.mp3");
                    while (Sound.IsPlaying)
                    {
                        Thread.Sleep(1);
                    }

                    Console.WriteLine("Recording");
                    _state.State = StateType.Recording;
                    Sound.StartRecording();
                }
                else if ((Sound.IsRecording) && (buttonEventArgs.ButtonEventType == ButtonEventType.Up))
                {
                    Sound.StopRecording();
                    // Need to wait for the recording to fully finish
                    // As it runs chunks of 1 second
                    while (Sound.IsRecording)
                    {
                        Thread.Sleep(1);
                    }

                    _state.State = StateType.Playing;
                    Sound.Play($"{_path}/sounds/acquired.mp3");
                    //while (Sound.IsPlaying)
                    //{
                    //    Thread.Sleep(1);
                    //}

                    //Sound.Play($"{_path}/record.wav");
                    _state.State = StateType.Idle;
                    // Now do the recognition
                    var phrase = Recognize($"{_path}/record.wav");
                    Console.WriteLine($"Prase recognized: {phrase}");
                    if (!string.IsNullOrEmpty(phrase))
                    {
                        var asr = GetTopIntent(phrase);
                        Console.WriteLine($"Top Intent: {asr}");
                        if (!string.IsNullOrEmpty(asr))
                        {
                            foreach (var evts in _eventId)
                            {
                                if (evts.Value.EventType.Contains(EventType.Asr))
                                {
                                    if (evts.Key.Connected)
                                    {
                                        Console.WriteLine($"Sending event asr to: {evts.Key.RemoteEndPoint}");
                                        AsrEvent asrEvent = new AsrEvent() { Time = DateTime.Now.ToString("yyyy-MM-dd\"T\"HH:mm:ss.ffffffzzz") };
                                        Nlu nlu = new Nlu() { Intent = asr.ToLower() };
                                        asrEvent.Nlu = nlu;
                                        SendMessage(asrEvent, evts.Key);
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Not connected: {evts.Key.RemoteEndPoint}");
                                    }
                                }
                            }

                            if (asr.ToLower() == "clock")
                            {
                                var dt = DateTime.Now;
                                //Speak();
                                var toSay = string.Format(_settings.ClockPhrase, dt.Hour, dt.Minute, dt.Second);
                                string host = $"https://{_settings.CognitiveRegion}.tts.speech.microsoft.com/cognitiveservices/v1";
                                string endpoint = $"https://{_settings.CognitiveRegion}.api.cognitive.microsoft.com/sts/v1.0/issueToken";
                                Authentication auth = new Authentication(endpoint, _settings.CognitiveKey);
                                var ttsFilePath = $"{_path}/{SoundDirectory}/toplay.mp3";
                                SaveTextToSpeechFile(host, auth, toSay, _settings.PrefferedVoice, ttsFilePath).Wait();
                                while (Sound.IsPlaying)
                                {
                                    Thread.Sleep(1);
                                }
                                Sound.Play(ttsFilePath);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ups: {ex}");
            }
        }

        #region Process sockets and events

        private static void ListenToSockets()
        {
            if (_tcpListener.Pending())
            {
                var th = new Thread(() =>
                {
                    var listner = _tcpListener.AcceptSocket();
                    lock (_socketLock)
                    {
                        if (!_sockets.Contains(listner))
                        {
                            _sockets.Add(listner);
                        }
                    }
                    try
                    {
                        listner.ReceiveBufferSize = _buffer.Length;
                        _connectedClients++;
                        Console.WriteLine($"{listner.RemoteEndPoint}   {listner.LocalEndPoint}   {listner.Connected}  # Connections: {_connectedClients}");
                        // Is it a new Client?
                        while ((listner.Connected) && (_applicationRunning))
                        {
                            int received;
                            string readString = string.Empty;
                            do
                            {
                                received = listner.Receive(_buffer);
                                if (received > 0)
                                {
                                    readString += Encoding.UTF8.GetString(_buffer, 0, received);
                                }

                                // need to wait a bit as when more is coming, the Available has
                                // not enough time to get the bytes
                                Thread.Sleep(10);
                            } while (listner.Available > 0);

                            if (readString.Length > 0)
                            {
                                Console.WriteLine($"Received {readString.Length} bytes");
                                var retArray = readString.Replace("\r", "").Split('\n');
                                foreach (var ret in retArray)
                                {
                                    if (ret.Length < 4)
                                        continue;

                                    var res = JsonConvert.DeserializeObject<Paquet>(ret);
                                    Console.WriteLine($"Type of paquet: {res.Type}");
                                    Console.WriteLine($"{ret}");
                                    var uptime = System.Environment.TickCount / 1000;
                                    var response = new Response() { Status = Status.Ok, Connections = _connectedClients, Uptime = uptime };
                                    switch (res.Type)
                                    {
                                        case PaquetType.Information:
                                            var info = JsonConvert.DeserializeObject<Info>(ret);
                                            response.RequestId = info.RequestId;
                                            if (info.Animation == null)
                                            {
                                                response.Status = Status.Error;
                                                response.ErrorMessage = $"{nameof(Info.Animation)} can't be empty or null";
                                                response.ErrorClass = "Mega error!";
                                            }

                                            SendMessage(response, listner);
                                            lock (_socketLock)
                                            {
                                                _queueProcess.Enqueue(new Process() { PaquetType = PaquetType.Information, ToProcess = info });
                                            }
                                            break;
                                        case PaquetType.Ears:
                                            var ears = JsonConvert.DeserializeObject<Net.Models.Ears>(ret);
                                            response.RequestId = ears.RequestId;
                                            if (_state.State == StateType.Asleep)
                                            {
                                                response.Status = Status.Canceled;
                                            }

                                            SendMessage(response, listner);
                                            lock (_socketLock)
                                            {
                                                _queueProcess.Enqueue(new Process() { PaquetType = PaquetType.Ears, ToProcess = ears });
                                            }
                                            break;
                                        case PaquetType.Command:
                                            var cmd = JsonConvert.DeserializeObject<Command>(ret);
                                            response.RequestId = cmd.RequestId;
                                            if (cmd.Sequence == null)
                                            {
                                                response.Status = Status.Error;
                                                response.ErrorMessage = $"{nameof(Command.Sequence)} can't be empty or null";
                                                response.ErrorClass = "Mega error!";

                                            }
                                            else if (_state.State == StateType.Asleep)
                                            {
                                                response.Status = Status.Canceled;
                                            }

                                            SendMessage(response, listner);
                                            lock (_socketLock)
                                            {
                                                _queueProcess.Enqueue(new Process() { PaquetType = PaquetType.Command, ToProcess = cmd });
                                            }
                                            break;
                                        case PaquetType.Message:
                                            var msg = JsonConvert.DeserializeObject<Message>(ret);
                                            response.RequestId = msg.RequestId;
                                            if (msg.Body == null)
                                            {
                                                response.Status = Status.Error;
                                                response.ErrorMessage = $"{nameof(Message.Body)} can't be empty or null";
                                                response.ErrorClass = "Mega error!";
                                            }
                                            else if (_state.State == StateType.Asleep)
                                            {
                                                response.Status = Status.Canceled;
                                            }

                                            SendMessage(response, listner);
                                            lock (_socketLock)
                                            {
                                                _queueProcess.Enqueue(new Process() { PaquetType = PaquetType.Message, ToProcess = msg });
                                            }
                                            break;
                                        case PaquetType.Cancel:
                                            var cancel = JsonConvert.DeserializeObject<Cancel>(ret);
                                            response.RequestId = cancel.RequestId;
                                            if (String.IsNullOrEmpty(response.RequestId))
                                            {
                                                response.Status = Status.Error;
                                                response.ErrorMessage = $"{nameof(Cancel.RequestId)} can't be empty or null";
                                                response.ErrorClass = "Mega error!";
                                            }
                                            SendMessage(response, listner);
                                            break;
                                        case PaquetType.Wakeup:
                                            var wakeup = JsonConvert.DeserializeObject<Wakeup>(ret);
                                            response.RequestId = wakeup.RequestId;
                                            SendMessage(response, listner);
                                            ProcessWakeUp();
                                            break;
                                        case PaquetType.Sleep:
                                            var sleep = JsonConvert.DeserializeObject<Sleep>(ret);
                                            response.RequestId = sleep.RequestId;
                                            SendMessage(response, listner);
                                            lock (_socketLock)
                                            {
                                                _queueProcess.Enqueue(new Process() { PaquetType = PaquetType.Sleep, ToProcess = sleep });
                                            }
                                            break;
                                        case PaquetType.Mode:
                                            var mode = JsonConvert.DeserializeObject<EventMode>(ret);
                                            response.RequestId = mode.RequestId;
                                            SendMessage(response, listner);

                                            EventNotification eventNotification = new EventNotification() { RequestId = mode.RequestId };
                                            eventNotification.Mode = mode.Mode;
                                            eventNotification.EventType = mode.Events.ToArray();
                                            // To DO : manage the events
                                            if (_eventId.ContainsKey(listner))
                                            {
                                                if (mode.Events.Count() > 0)
                                                {
                                                    _eventId[listner] = eventNotification;
                                                }
                                                else
                                                {
                                                    _eventId.Remove(listner);
                                                }
                                            }
                                            else
                                            {
                                                if (mode.Events.Count() > 0)
                                                {
                                                    _eventId.Add(listner, eventNotification);
                                                }
                                            }

                                            break;
                                        case PaquetType.Test:
                                            var test = JsonConvert.DeserializeObject<TestMode>(ret);
                                            response.RequestId = test.RequestId;
                                            Console.WriteLine($"{nameof(test)}");
                                            SendMessage(response, listner);
                                            lock (_socketLock)
                                            {
                                                _queueProcess.Enqueue(new Process() { PaquetType = PaquetType.Test, ToProcess = test });
                                            }
                                            break;
                                        case PaquetType.Statistics:
                                            var stats = JsonConvert.DeserializeObject<Statistics>(ret);
                                            stats.Type = PaquetType.Response;
                                            stats.Hardware = new Hardware();
                                            stats.Hardware.IsRfid = false;
                                            var pos = Ears.DetectEarPosition(Ear.Left);
                                            stats.Hardware.LeftEarStatus = $"Position {pos}";
                                            pos = Ears.DetectEarPosition(Ear.Right);
                                            stats.Hardware.RrightEarStatus = $"Position {pos}";
                                            var sound = File.ReadAllText("/proc/asound/cards");
                                            if (sound.Contains("tagtagtagsound"))
                                            {
                                                stats.Hardware.SoundCard = "tagtagtagsound";
                                                stats.Hardware.SoundInput = true;
                                            }
                                            else if (sound.Length > 5)
                                            {
                                                stats.Hardware.SoundInput = true;
                                                stats.Hardware.SoundCard = sound;
                                            }
                                            else
                                            {
                                                stats.Hardware.SoundInput = false;
                                            }

                                            stats.Hardware.Model = "2019_TAG or more";
                                            stats.State = _state.State;
                                            stats.Connections = _connectedClients;
                                            stats.Uptime = uptime;
                                            SendMessage(stats, listner);
                                            break;
                                        // Only nabd can emmit those ones
                                        case PaquetType.Response:
                                        case PaquetType.EarsEvent:
                                        case PaquetType.ButtonEvent:
                                        case PaquetType.State:
                                        default:
                                            break;
                                    }
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex}");
                    }
                    lock (_socketLock)
                    {
                        _sockets.Remove(listner);
                    }
                    _connectedClients--;
                });
                th.Priority = ThreadPriority.Lowest;
                //lock (_threadListLock)
                //{
                //    _listnerThread.Add(th);
                //}
                th.Start();
            }
        }

        private static void ProcessWakeUp()
        {
            WakeUp();
            _state.State = StateType.Idle;
            BoradcastState();
        }

        private static void ProcessMessage(Message msg)
        {
            // Play the sequence if nothing else is playing, or wait
            // And cancel if expiration date is past

            DateTime dtCancelMsg = msg.Expiration.GetValueOrDefault();
            bool expired = false;
            while (IsBusy)
            {
                if (dtCancelMsg > DateTime.Now)
                {
                    expired = true;
                    break;
                }
            }

            if (!expired)
            {
                _state.State = StateType.Playing;
                BoradcastState();
                if (msg.Signature != null)
                {
                    foreach (var audio in msg.Signature.AudioList)
                    {
                        PlayAndWait(FindMusic(audio));
                    }
                }

                foreach (var body in msg.Body)
                {
                    if (body.ChoreographyList != null)
                    {
                        Choreography.ReadChoreography(FindChoreography(body.ChoreographyList));
                        while (IsBusy)
                        {
                            Thread.Sleep(100);
                        }
                    }

                    foreach (var audio in body?.AudioList)
                    {
                        PlayAndWait(FindMusic(audio));
                    }
                }

                _state.State = StateType.Idle;
                BoradcastState();
            }
        }

        private static void ProcessGoToSleep()
        {
            _state.State = StateType.Asleep;
            BoradcastState();
            GoToSleep();
        }

        private static void ProcessTest(TestMode test)
        {
            var backupState = _state.State;
            _state.State = StateType.Playing;
            BoradcastState();
            if (test.Test == TestType.Ears)
            {
                TestEars();
            }
            else
            {
                TestLeds();
            }

            _state.State = backupState;
            BoradcastState();
        }

        private static void ProcessCommand(Command cmd)
        {
            // Play the sequence if nothing else is playing, or wait
            // And cancel if expiration date is past

            DateTime dtCancel = cmd.Expiration.GetValueOrDefault();
            bool expired = false;
            while (IsBusy)
            {
                if (dtCancel > DateTime.Now)
                {
                    expired = true;
                    break;
                }
            }

            if (!expired)
            {
                _state.State = StateType.Playing;
                BoradcastState();
                foreach (var cor in cmd.Sequence)
                {
                    if (cor.ChoreographyList != null)
                    {
                        Choreography.ReadChoreography(FindChoreography(cor.ChoreographyList));
                    }

                    if (cor.AudioList != null)
                    {
                        foreach (var audio in cor.AudioList)
                        {
                            PlayAndWait(FindMusic(audio));
                        }
                    }
                }
                _state.State = StateType.Idle;
                BoradcastState();
            }
        }

        private static void ProcessMoveEar(Net.Models.Ears ears)
        {
            WaitForIdle();
            Ears.MoveAbsolute(Ear.Left, EarDirection.Forward, (byte)ears.Left);
            Ears.MoveAbsolute(Ear.Right, EarDirection.Forward, (byte)ears.Right);
            WaitForIdle();
        }

        private static void ProcessInformation(Info info)
        {
            lock (_animationLock)
            {
                _animation = info.Animation;
            }
        }



        private static void BoradcastState()
        {
            var json = JsonConvert.SerializeObject(_state);
            lock (_socketLock)
            {
                var numSockets = _sockets.Count();
                for (int i = 0; i < numSockets; i++)
                {
                    SendMessage(_state, _sockets[i]);
                }
            }

        }

        private static void SendMessage(object objToSend, Socket listener)
        {
            var json = JsonConvert.SerializeObject(objToSend) + "\r\n";
            byte[] buff = Encoding.UTF8.GetBytes(json);
            listener.BeginSend(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(EndSend), listener);
            Console.WriteLine($"Sent response: {json}");
        }

        private static void EndSend(IAsyncResult ar)
        {
            // Retrieve the socket from the state object.  
            Socket handler = (Socket)ar?.AsyncState;
            try
            {
                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                //Console.WriteLine("Sent {0} bytes to client.", bytesSent);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Removing one endpoint");
                if (handler != null)
                {
                    _sockets.Remove(handler);
                }
            }
        }

        static private void WaitForIdle()
        {
            while (IsBusy)
            {
                Thread.Sleep(1);
            }
        }

        #endregion

        #region Recognize ASR

        private static async Task SaveTextToSpeechFile(string host, Authentication auth, string input, Voice voice, string path)
        {
            using (HttpClient client = new HttpClient())
            {
                string accessToken;
                try
                {
                    accessToken = auth.GetAccessToken();
                }
                catch (Exception ex)
                {
                    return;
                }

                // Create SSML document.
                XDocument body = new XDocument(
                        new XElement("speak",
                            new XAttribute("version", "1.0"),
                            new XAttribute(XNamespace.Xml + "lang", $"{voice.Locale}"),
                            new XElement("voice",
                                new XAttribute(XNamespace.Xml + "lang", $"{voice.Locale}"),
                                new XAttribute(XNamespace.Xml + "gender", $"{voice.Gender}"),
                                new XAttribute("name", $"{voice.ShortName}"),
                                input)));

                using (HttpRequestMessage request = new HttpRequestMessage())
                {
                    // Set the HTTP method
                    request.Method = HttpMethod.Post;
                    // Construct the URI
                    request.RequestUri = new Uri(host);
                    // Set the content type header
                    request.Content = new StringContent(body.ToString(), Encoding.UTF8, "application/ssml+xml");
                    // Set additional header, such as Authorization and User-Agent
                    request.Headers.Add("Authorization", "Bearer " + accessToken);
                    request.Headers.Add("User-Agent", "Nabaztag");
                    request.Headers.Add("Connection", "Keep-Alive");
                    // Audio output format. See API reference for full list.
                    request.Headers.Add("X-Microsoft-OutputFormat", "audio-16khz-32kbitrate-mono-mp3");
                    using (HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false))
                    {
                        response.EnsureSuccessStatusCode();

                        // Asynchronously read the response
                        using (Stream dataStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
                            {
                                await dataStream.CopyToAsync(fileStream).ConfigureAwait(false);
                                fileStream.Close();
                            }
                        }
                    }
                }
            }
        }

        private static string Recognize(string fileName)
        {
            try
            {
                // &format=detailed
                var requestUri = new Uri($"https://{_settings.CognitiveRegion}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language={_settings.Locale}");
                var request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                request.SendChunked = true;
                request.Accept = @"application/json;text/xml";
                request.Method = "POST";
                request.ProtocolVersion = HttpVersion.Version11;
                //request.Host = "";
                request.ContentType = @"audio/wav; codecs=audio/pcm; samplerate=16000";
                request.Headers["Ocp-Apim-Subscription-Key"] = _settings.CognitiveKey; // "5e0b011c63c644cc956f60b50add0e70";
                request.AllowWriteStreamBuffering = false;

                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    // Open a request stream and write 1024 byte chunks in the stream one at a time.
                    byte[] buffer = null;
                    int bytesRead = 0;
                    using (var requestStream = request.GetRequestStream())
                    {
                        // Read 1024 raw bytes from the input audio file.
                        buffer = new Byte[checked((uint)Math.Min(1024, (int)fs.Length))];
                        while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            requestStream.Write(buffer, 0, bytesRead);
                        }

                        requestStream.Flush();
                    }
                }

                var response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseString = reader.ReadToEnd();
                    var recognize = JsonConvert.DeserializeObject<RecognizeText>(responseString);
                    if (recognize.RecognitionStatus.ToLower() == "success")
                    {
                        if (recognize.NBest != null)
                        {
                            Console.WriteLine($"You said with confidence of {recognize.NBest?[0].Confidence.ToString("0.00")}: {recognize.NBest?[0].Display}");
                            return recognize.NBest?[0].Display;
                        }
                        else
                        {
                            Console.WriteLine($"You said: {recognize.DisplayText}");
                            return recognize.DisplayText;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Sorry, didn't recognize anything");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return string.Empty;
        }

        private static string GetTopIntent(string toAnalyze)
        {
            try
            {
                var requestUri = new Uri($"https://{_settings.LuisRegion}.api.cognitive.microsoft.com/luis/prediction/v3.0/apps/{_settings.LuisAddId}/slots/{_settings.LuisStage}/predict?subscription-key={_settings.LuisKey}&verbose=false&show-all-intents=false&log=false&query={toAnalyze}");
                var request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                var response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseString = reader.ReadToEnd();
                    var recognize = JsonConvert.DeserializeObject<Luis>(responseString);
                    return recognize.prediction.topIntent;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private static void Speak(string toSay)
        {
            try
            {
                var requestUri = new Uri($"{_settings.TtsUrl}/{HttpUtility.UrlEncode(toSay)}");
                var request = (HttpWebRequest)HttpWebRequest.Create(requestUri);
                var response = request.GetResponse();
                using (Stream stream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseString = reader.ReadToEnd();
                    Console.WriteLine($"Speak result: {responseString}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Can't speak: {ex}");
            }
        }

        #endregion

        #region Music and Choreographies

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        private static void PlayAnimation(Animation animation)
        {
            if (animation == null)
            {
                return;
            }
            //Console.WriteLine($"Playing animation");
            var tempoMilliseconds = animation.Tempo * 10;

            if ((!IsBusy) && (_state.State == StateType.Idle))
            {
                foreach (var col in animation.Colors)
                {
                    //Console.WriteLine($"Playing animation");
                    DateTime dtTempo = DateTime.Now.AddMilliseconds(tempoMilliseconds);
                    if (!string.IsNullOrEmpty(col.Bottom))
                    {
                        var color = StringToByteArray(col.Bottom);
                        //Console.WriteLine($"{color[0]}, {color[1]}, {color[2]}");
                        if (color.Length == 3)
                        {
                            Leds.SetLed(Led.Bottom, Color.FromArgb(color[0], color[1], color[2]));
                        }
                    }
                    if (!string.IsNullOrEmpty(col.Center))
                    {
                        var color = StringToByteArray(col.Center);
                        //Console.WriteLine($"{color[0]}, {color[1]}, {color[2]}");
                        if (color.Length == 3)
                        {
                            Leds.SetLed(Led.Center, Color.FromArgb(color[0], color[1], color[2]));
                        }
                    }
                    if (!string.IsNullOrEmpty(col.Left))
                    {
                        var color = StringToByteArray(col.Left);
                        //Console.WriteLine($"{color[0]}, {color[1]}, {color[2]}");
                        if (color.Length == 3)
                        {
                            Leds.SetLed(Led.Left, Color.FromArgb(color[0], color[1], color[2]));
                        }
                    }
                    if (!string.IsNullOrEmpty(col.Right))
                    {
                        var color = StringToByteArray(col.Right);
                        //Console.WriteLine($"{color[0]}, {color[1]}, {color[2]}");
                        if (color.Length == 3)
                        {
                            Leds.SetLed(Led.Right, Color.FromArgb(color[0], color[1], color[2]));
                        }
                    }
                    if (!string.IsNullOrEmpty(col.Nose))
                    {
                        var color = StringToByteArray(col.Nose);
                        //Console.WriteLine($"{color[0]}, {color[1]}, {color[2]}");
                        if (color.Length == 3)
                        {
                            Leds.SetLed(Led.Nose, Color.FromArgb(color[0], color[1], color[2]));
                        }
                    }

                    Leds.Render();
                    while (dtTempo > DateTime.Now)
                    {
                        Thread.Sleep(1);
                    }
                }
            }
        }

        private static void PlayAndWait(string fullName)
        {
            Sound.Play(fullName);
            while (Sound.IsPlaying)
            {
                Thread.Sleep(1);
            }
        }

        private static string FindMusic(string toFind)
        {
            // Directory structure for the sounds in the pynab strcuture
            // /home/pi/pynab/nabXXX/sounds/nabXXX/file.mp3
            // /home/pi/pynab/nabXXX/sounds/fr-FR/nabXXX/file.mp3
            // /home/pi/pynab/nabXXX/sounds/fr-FR/nabXXX/othdir/file.mp3
            // File pattern arrives like this: nabXXX/file.mp3
            // Randome file in a specific directory: nabXXX/*.mp3
            // Randome file in a specific directory: nabXXX/otherdir/*.mp3

            // First search in the directory without locale
            try
            {
                var dirToFind = toFind.Substring(0, toFind.LastIndexOf('/'));
                var appName = toFind.Substring(0, toFind.IndexOf('/'));
                var di = new DirectoryInfo($"{_path}/../{PyNabDirectory}/{appName}/{SoundDirectory}/{dirToFind}");
                var searchPAttern = toFind.Substring(toFind.LastIndexOf('/') + 1);
                var files = di.GetFiles(searchPAttern, SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    return files[new Random().Next(files.Length - 1)].FullName;
                }

            }
            catch
            { }

            // Then search in the directory with locale
            try
            {
                var dirToFind = toFind.Substring(0, toFind.LastIndexOf('/'));
                var appName = toFind.Substring(0, toFind.IndexOf('/'));
                var di = new DirectoryInfo($"{_path}/../{PyNabDirectory}/{appName}/{SoundDirectory}/{_locale}/{dirToFind}");
                var searchPAttern = toFind.Substring(toFind.LastIndexOf('/') + 1);
                var files = di.GetFiles(searchPAttern, SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    return files[new Random().Next(files.Length - 1)].FullName;
                }
            }
            catch
            { }

            // Try absolute path
            try
            {
                var dirToFind = toFind.Substring(0, toFind.LastIndexOf('/'));
                var di = new DirectoryInfo($"{dirToFind}");
                var searchPAttern = toFind.Substring(toFind.LastIndexOf('/') + 1);
                var files = di.GetFiles(searchPAttern, SearchOption.AllDirectories);
                if (files.Length > 0)
                {
                    return files[new Random().Next(files.Length - 1)].FullName;
                }
            }
            catch
            { }

            // Try for ../../../../somewhere/file.mp3
            try
            {
                var dirToFind = toFind.Substring(0, toFind.LastIndexOf('/'));
                var di = new DirectoryInfo($"{_path}/../{PyNabDirectory}/nabd/{SoundDirectory}/fr_FR/{dirToFind}");
                var searchPAttern = toFind.Substring(toFind.LastIndexOf('/') + 1);
                var files = di.GetFiles(searchPAttern, SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    return files[new Random().Next(files.Length - 1)].FullName;
                }
            }
            catch
            { }


            // Finally seach in the main application directories
            try
            {
                var di = new DirectoryInfo($"{_path}/{SoundDirectory}");
                var searchPAttern = toFind.Substring(toFind.LastIndexOf('/') + 1);
                var files = di.GetFiles(searchPAttern, SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    return files[new Random().Next(files.Length - 1)].FullName;
                }
            }
            catch
            { }

            // let's search just for the last part in all the root directories
            try
            {
                var di = new DirectoryInfo($"{_path}/..");
                var searchPAttern = toFind.Substring(toFind.LastIndexOf('/') + 1);
                var files = di.GetFiles(searchPAttern, SearchOption.AllDirectories);
                if (files.Length > 0)
                {
                    return files[new Random().Next(files.Length - 1)].FullName;
                }
            }
            catch
            { }

            return string.Empty;
        }

        private static string FindChoreography(string toFind)
        {
            // Directory structure for the sounds in the pynab strcuture
            // /home/pi/pynab/nabXXX/choreographies/nabXXX/file.chor
            // File pattern arrives like this: nabXXX/file.chor
            // Randome file in a specific directory: nabXXX/*.chor

            // First search into the directory
            try
            {
                var dirToFind = toFind.Substring(0, toFind.LastIndexOf('/'));
                var appName = toFind.Substring(0, toFind.IndexOf('/'));
                var di = new DirectoryInfo($"{_path}/../{PyNabDirectory}/{appName}/{ChoreographyDirectory}/{dirToFind}");
                var searchPAttern = toFind.Substring(toFind.LastIndexOf('/') + 1);
                var files = di.GetFiles(searchPAttern, SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    return files[new Random().Next(files.Length - 1)].FullName;
                }

            }
            catch
            { }

            // Try absolute path
            try
            {
                var dirToFind = toFind.Substring(0, toFind.LastIndexOf('/'));
                var di = new DirectoryInfo($"{dirToFind}");
                var searchPAttern = toFind.Substring(toFind.LastIndexOf('/') + 1);
                var files = di.GetFiles(searchPAttern, SearchOption.AllDirectories);
                if (files.Length > 0)
                {
                    return files[new Random().Next(files.Length - 1)].FullName;
                }
            }
            catch
            { }

            // Finally seach in the main application directories
            try
            {
                var di = new DirectoryInfo($"{_path}/{ChoreographyDirectory}");
                var searchPAttern = toFind.Substring(toFind.LastIndexOf('/') + 1);
                var files = di.GetFiles(searchPAttern, SearchOption.TopDirectoryOnly);
                if (files.Length > 0)
                {
                    return files[new Random().Next(files.Length - 1)].FullName;
                }
            }
            catch
            { }

            return string.Empty;
        }

        #endregion

        #region Tests

        private static void TestEars()
        {
            Ears.MoveAbsolute(Ear.Left, EarDirection.Backward, 0, true);
            Ears.MoveAbsolute(Ear.Right, EarDirection.Forward, 0, true);
            while (Ears.IsAnyMoving)
            {
                Task.Delay(1);
            }

            for (byte i = 0; i < 17; i++)
            {
                Ears.MoveRelative(Ear.Left, EarDirection.Forward, 1, true);
                Ears.MoveRelative(Ear.Right, EarDirection.Backward, 1, true);
                while (Ears.IsAnyMoving)
                {
                    Task.Delay(1);
                }
            }
        }

        private static void TestLeds()
        {
            List<Color> colors = new List<Color>()
            {
                Color.FromArgb(0, 0, 0),
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(0, 255, 0),
                Color.FromArgb(0, 0, 255),
                Color.FromArgb(255, 255, 255),
                Color.FromArgb(127, 127, 127),
                Color.FromArgb(0, 0, 0),
            };

            foreach (var col in colors)
            {
                for (int i = 0; i < 4; i++)
                {
                    Leds.SetLedAndDisplay((Led)i, col);
                    Thread.Sleep(200);
                }
            }
        }

        #endregion

        #region Sleep and Awake

        private static void GoToSleep()
        {
            //_animationIdle.Abort();
            Ears.MoveAbsolute(Ear.Left, EarDirection.Forward, 0);
            Ears.MoveAbsolute(Ear.Right, EarDirection.Forward, 0);
            Leds.SetAllLeds(Color.Black);
        }

        private static void WakeUp()
        {
            Ears.MoveAbsolute(Ear.Left, EarDirection.Backward, 10, true);
            Ears.MoveAbsolute(Ear.Right, EarDirection.Backward, 10, true);
            TestLeds();
        }

        #endregion
    }
}
