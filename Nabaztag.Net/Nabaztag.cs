// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Nabaztag.Net.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nabaztag.Net
{
    public class Nabaztag
    {
        private const int DefaultTcpPortEmmit = 10543;
        private const int MaxWaitingTimeToReconnect = 30_000;
        private Dictionary<string, Response> _LastRequestId = new Dictionary<string, Response>();
        private TcpClient _TcpClient;
        private string _hostName;
        private int _port;

        // public event s
        public delegate void EarsEventHandler(object sender, EarsEvent ears);
        public event EarsEventHandler EarsEvent;
        public delegate void ButtonEventHandler(object sender, ButtonEvent buttonEvent);
        public event ButtonEventHandler ButtonEvent;
        public delegate void StateEventHandler(object sender, NabState state);
        public event StateEventHandler StateEvent;
        public delegate void AsrEventHandler(object sender, AsrEvent state);
        public event AsrEventHandler AsrEvent;

        /// <summary>
        /// SStatistics on uptime, hardware, model
        /// </summary>
        public Statistics Statistics { get; internal set; }

        /// <summary>
        /// The state of the Nabaztag
        /// </summary>
        public NabState State { get; internal set; }

        public Nabaztag() : this("localhost", DefaultTcpPortEmmit)
        { }

        public Nabaztag(string hostName, int tcpPort)
        {
            _hostName = hostName;
            _port = tcpPort;
            Statistics = new Statistics();
            TryToConnect();
            Task.Factory.StartNew(() => ProcessIncoming());
        }

        private bool IsConnected()
        {
            if (_TcpClient == null)
            {
                return false;
            }

            return _TcpClient.Connected;
        }

        private void TryToConnect()
        {
            int waitingTime = 100;

            while (!IsConnected())
            {
                try
                {
                    _TcpClient = new TcpClient(_hostName, _port);
                }
                catch
                {
                    waitingTime += 100;
                    if (waitingTime > MaxWaitingTimeToReconnect)
                    {
                        waitingTime = MaxWaitingTimeToReconnect;
                    }
                    Thread.Sleep(waitingTime);
                }
            }
        }

        /// <summary>
        /// Wakeup the Nabaztag
        /// </summary>
        /// <param name="needRequestId">true if you want a confirmation, by default, yes</param>
        /// <param name="cancelAfterSeconds">Cancel waiting for the answer after the seconds defined. By default, it will wait indefinitely</param>
        /// <returns>Response object</returns>
        public Response Wakeup(bool needRequestId = true, int cancelAfterSeconds = -1)
        {
            var wakeup = new Wakeup();
            Guid reqId;
            if (needRequestId)
            {
                reqId = Guid.NewGuid();
                _LastRequestId.Add(reqId.ToString(), null);
                wakeup.RequestId = reqId.ToString();
            }
            return SendMessageProcessResponse(JsonConvert.SerializeObject(wakeup), reqId, cancelAfterSeconds);
        }

        /// <summary>
        /// Tell the Nabaztag to sleep
        /// </summary>
        /// <param name="needRequestId">true if you want a confirmation, by default, yes</param>
        /// <param name="cancelAfterSeconds">Cancel waiting for the answer after the seconds defined. By default, it will wait indefinitely</param>
        /// <returns>Response object</returns>
        public Response Sleep(bool needRequestId = true, int cancelAfterSeconds = -1)
        {
            var sleep = new Sleep();
            Guid reqId;
            if (needRequestId)
            {
                reqId = Guid.NewGuid();
                _LastRequestId.Add(reqId.ToString(), null);
                sleep.RequestId = reqId.ToString();
            }
            return SendMessageProcessResponse(JsonConvert.SerializeObject(sleep), reqId, cancelAfterSeconds);
        }

        /// <summary>
        /// Send a command with specific choreography(ies) and audio(s)
        /// </summary>
        /// <param name="sequence">A sequence class</param>
        /// <param name="needRequestId">true if you want a confirmation, by default, yes</param>
        /// <param name="cancelAfterSeconds">Cancel waiting for the answer after the seconds defined. By default, it will wait indefinitely</param>
        /// <returns>Response object</returns>
        public Response Command(Sequence[] sequence, bool needRequestId = true, int cancelAfterSeconds = -1)
        {
            var command = new Command();
            Guid reqId;
            if (needRequestId)
            {
                reqId = Guid.NewGuid();
                _LastRequestId.Add(reqId.ToString(), null);
                command.RequestId = reqId.ToString();
            }
            command.Sequence = sequence;
            return SendMessageProcessResponse(JsonConvert.SerializeObject(command), reqId, cancelAfterSeconds);
        }

        /// <summary>
        /// Send a message command
        /// </summary>
        /// <param name="signature">the signature sequence, audio will be played first</param>
        /// <param name="body">the body sequence</param>
        /// <param name="expiracy">Expriracy time, DateTime.MinValue for no expriracy</param>
        /// <param name="needRequestId">true if you want a confirmation, by default, yes</param>
        /// <param name="cancelAfterSeconds">Cancel waiting for the answer after the seconds defined. By default, it will wait indefinitely</param>
        /// <returns>Response object</returns>
        public Response Message(Sequence signature, Sequence[] body, DateTime expiracy, bool needRequestId = true, int cancelAfterSeconds = -1)
        {
            var command = new Message();
            Guid reqId;
            if (needRequestId)
            {
                reqId = Guid.NewGuid();
                _LastRequestId.Add(reqId.ToString(), null);
                command.RequestId = reqId.ToString();
            }

            if (expiracy != DateTime.MinValue)
            {
                command.Expiration = expiracy;
            }

            command.Signature = signature;
            command.Body = body;
            return SendMessageProcessResponse(JsonConvert.SerializeObject(command), reqId, cancelAfterSeconds);
        }

        /// <summary>
        /// Send a message command
        /// </summary>
        /// <param name="message">The message class to send</param>
        /// <param name="needRequestId">true if you want a confirmation, by default, yes</param>
        /// <param name="cancelAfterSeconds">Cancel waiting for the answer after the seconds defined. By default, it will wait indefinitely</param>
        /// <returns></returns>
        public Response Message(Message message, bool needRequestId = true, int cancelAfterSeconds = -1)
        {
            Guid reqId;
            if (needRequestId)
            {
                reqId = Guid.NewGuid();
                _LastRequestId.Add(reqId.ToString(), null);
                message.RequestId = reqId.ToString();
            }

            return SendMessageProcessResponse(JsonConvert.SerializeObject(message), reqId, cancelAfterSeconds);
        }

        /// <summary>
        /// Set the mode for the application
        /// </summary>
        /// <param name="modeType">the mode to set the service in</param>
        /// <param name="eventType">set the event types and call backs</param>
        /// <param name="needRequestId">true if you want a confirmation, by default, yes</param>
        /// <param name="cancelAfterSeconds">Cancel waiting for the answer after the seconds defined. By default, it will wait indefinitely</param>
        /// <returns>Response object</returns>
        public Response EventMode(ModeType modeType, EventType[] eventType, bool needRequestId = true, int cancelAfterSeconds = -1)
        {
            var eventMode = new EventMode();
            Guid reqId;
            if (needRequestId)
            {
                reqId = Guid.NewGuid();
                _LastRequestId.Add(reqId.ToString(), null);
                eventMode.RequestId = reqId.ToString();
            }

            eventMode.Events = eventType;
            eventMode.Mode = modeType;
            var ser = JsonConvert.SerializeObject(eventMode);
            ser = ser.ToLower();
            return SendMessageProcessResponse(ser, reqId, cancelAfterSeconds);
        }

        /// <summary>
        /// Cancel a request id
        /// </summary>
        /// <param name="requestId">the request id to cancel</param>
        /// <returns>Response object</returns>
        public Response Cancel(string requestId)
        {
            Cancel cancel = new Cancel() { RequestId = requestId };
            // We don't need a guid and we don't need to wait
            return SendMessageProcessResponse(JsonConvert.SerializeObject(cancel), new Guid(), -1);
        }

        /// <summary>
        /// Send a Test command, this moved the ears and rotate colors
        /// </summary>
        /// <param name="testType">Ears or Leds</param>
        /// <param name="needRequestId">true if you want a confirmation, by default, yes</param>
        /// <param name="cancelAfterSeconds">Cancel waiting for the answer after the seconds defined. By default, it will wait indefinitely</param>
        /// <returns>Response object</returns>
        public Response Test(TestType testType, bool needRequestId = true, int cancelAfterSeconds = -1)
        {
            TestMode test = new TestMode() { Test = testType };
            Guid reqId;
            if (needRequestId)
            {
                reqId = Guid.NewGuid();
                _LastRequestId.Add(reqId.ToString(), null);
                test.RequestId = reqId.ToString();
            }

            return SendMessageProcessResponse(JsonConvert.SerializeObject(test), reqId, cancelAfterSeconds);
        }

        /// <summary>
        /// Send an info command
        /// </summary>
        /// <param name="info"></param>
        /// <param name="needRequestId"></param>
        /// <param name="cancelAfterSeconds"></param>
        /// <returns></returns>
        public Response Info(Info info, bool needRequestId = true, int cancelAfterSeconds = -1)
        {
            Guid reqId;
            if (needRequestId)
            {
                reqId = Guid.NewGuid();
                _LastRequestId.Add(reqId.ToString(), null);
                info.RequestId = reqId.ToString();
            }

            var ret = JsonConvert.SerializeObject(info);
            Console.WriteLine($"{ret.Length}  {ret}");

            return SendMessageProcessResponse(JsonConvert.SerializeObject(info), reqId, cancelAfterSeconds);
        }

        private bool SendMessage(string message)
        {
            if (!_TcpClient.Connected)
            {
                TryToConnect();
                //return false;
            }
            message += "\r\n";
            int ret;
            try
            {
                ret = _TcpClient.Client.Send(Encoding.UTF8.GetBytes(message));
            }
            catch 
            {
                // Try one more time
                _TcpClient.Dispose();
                _TcpClient = null;
                TryToConnect();
                ret = _TcpClient.Client.Send(Encoding.UTF8.GetBytes(message));
            }
            
            return (ret > 0);
        }

        private Response SendMessageProcessResponse(string toSend, Guid reqId, int cancelAfterSeconds)
        {
            var msgok = SendMessage(toSend);
            if (!msgok)
                return new Response() { Status = Status.Error, ErrorClass = "Sending message", ErrorMessage = "Error sending message, check you have TCP/IP connection" };
            if (reqId == Guid.Empty)
                return new Response() { Status = Status.Ok };
            DateTime exp = DateTime.Now.AddSeconds(cancelAfterSeconds);
            bool isExpired = false;
            while ((_LastRequestId[reqId.ToString()] == null)
                && (!isExpired))
            {
                isExpired = (exp < DateTime.Now) && (cancelAfterSeconds > 0);
            }
            if (isExpired)
            {
                _LastRequestId.Remove(reqId.ToString());
                Debug.WriteLine("Expiration");
                return new Response() { Status = Status.Expired };
            }
            Debug.WriteLine($"Response received: {_LastRequestId[reqId.ToString()].Status}");
            return _LastRequestId[reqId.ToString()];
        }

        public Statistics GetStatistics()
        {
            Paquet paquet = new Paquet() { Type = PaquetType.Statistics };
            Guid reqId;

            reqId = Guid.NewGuid();
            _LastRequestId.Add(reqId.ToString(), null);
            paquet.RequestId = reqId.ToString();

            var res = SendMessageProcessResponse(JsonConvert.SerializeObject(paquet), reqId, 30);
            return Statistics;
        }

        private void ProcessIncoming()
        {
            // Listen to tcp
            while (true)
            {
                var received = _TcpClient.Available;
                if (received > 0)
                {
                    byte[] buffer = new byte[received];
                    received = _TcpClient.GetStream().ReadAsync(buffer, 0, received).GetAwaiter().GetResult();
                    var res = Encoding.UTF8.GetString(buffer, 0, received); // text coming from TCP listener
                    res = res.Replace("\n", "");
                    var results = res.Split('\r');
                    foreach (var result in results)
                    {
                        if (!string.IsNullOrEmpty(result))
                        {
                            Console.WriteLine(result);
                            try
                            {
                                var ret = JsonConvert.DeserializeObject<Paquet>(result);
                                switch (ret.Type)
                                {
                                    case PaquetType.State:
                                        State = JsonConvert.DeserializeObject<NabState>(result);
                                        Statistics.State = State.State;
                                        StateEvent?.Invoke(this, State);
                                        break;
                                    case PaquetType.EarsEvent:
                                    case PaquetType.EarEvent:
                                        var earsEvent = JsonConvert.DeserializeObject<EarsEvent>(result);
                                        EarsEvent?.Invoke(this, earsEvent);
                                        break;
                                    case PaquetType.ButtonEvent:
                                        var buttonEvent = JsonConvert.DeserializeObject<ButtonEvent>(result);
                                        ButtonEvent?.Invoke(this, buttonEvent);
                                        break;
                                    case PaquetType.Response:
                                        var response = JsonConvert.DeserializeObject<Response>(result);
                                        if (response.Status == null)
                                        {
                                            // We had a statistics in there
                                            Statistics.Hardware = response.Hardware;
                                            Statistics.State = response.State;
                                            Statistics.Uptime = response.Uptime;
                                            Statistics.Connections = response.Connections;
                                        }

                                        if (response.RequestId != null)
                                        {
                                            // If the Response.Status is null then it's the answer to the statistics
                                            _LastRequestId[response.RequestId] = response.Status == null ? new Response() { Status = Status.Ok } : response;
                                        }

                                        break;
                                    case PaquetType.AsrEvent:
                                        var asrEvent = JsonConvert.DeserializeObject<AsrEvent>(result);
                                        AsrEvent?.Invoke(this, asrEvent);
                                        break;
                                    case PaquetType.Statistics:
                                        Statistics = JsonConvert.DeserializeObject<Statistics>(result);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }
                        }
                    }
                }
            }
        }
    }
}
