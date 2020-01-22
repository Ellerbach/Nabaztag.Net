using Nabaztag.Net.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Nabaztag.Simulator
{
    class Program
    {
        private static TcpListener tcpListener;
        private static List<Socket> tcpClientsList = new List<Socket>();
        private static NabState state = new NabState() { State = StateType.Asleep };
        private static Dictionary<string, EventType[]> eventId = new Dictionary<string, EventType[]>();

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Nabaztag Simulator");
            tcpListener = new TcpListener(IPAddress.Any, 10543);
            tcpListener.Start();

            byte[] buffer = new byte[7000];

            // Send status every 20 seconds
            const int updateStatus = 20;
            DateTime statusUpdate = DateTime.Now;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (DateTime.Now > statusUpdate)
                    {
                        foreach (var tcp in tcpClientsList)
                        {
                            if (tcp.Connected)
                                tcp.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(state) + "\r\n"), SocketFlags.Broadcast);
                        }
                        statusUpdate = DateTime.Now.AddSeconds(updateStatus);
                    }
                }
            });

            //Simulate couple of events every time to time
            const int updateEvents = 30;
            DateTime eventsUpdate = DateTime.Now;
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (DateTime.Now > statusUpdate)
                    {
                        foreach (var tcp in tcpClientsList)
                        {
                            if (tcp.Connected)
                            {
                                foreach (var lst in eventId)
                                {
                                    foreach (var eve in lst.Value)
                                    {
                                        if (eve == EventType.Button)
                                            tcp.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ButtonEvent() { Event = ButtonEventType.Click }) + "\r\n"), SocketFlags.Broadcast);
                                        if (eve == EventType.Ears)
                                            tcp.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new EarsEvent() { Left = new Random().Next(), Right = new Random(new Random().Next()).Next() }) + "\r\n"), SocketFlags.Broadcast);
                                    }
                                }
                            }
                        }
                        eventsUpdate = DateTime.Now.AddSeconds(updateEvents);
                    }
                }
            });

            while (!Console.KeyAvailable)
            {
                var listener = tcpListener.AcceptSocket();
                // Is it a new Client?
                if (!tcpClientsList.Contains(listener))
                    tcpClientsList.Add(listener);
                var received = listener.Receive(buffer);
                if (received > 0)
                {
                    Console.WriteLine($"Received {received} bytes");
                    var retArray = Encoding.UTF8.GetString(buffer, 0, received).Split("\r\n");
                    foreach (var ret in retArray)
                    {
                        try
                        {
                            var res = JsonConvert.DeserializeObject<Paquet>(ret);
                            Console.WriteLine($"Type of paquet: {res.Type}");
                            var response = new Response() { Status = Status.Ok };
                            switch (res.Type)
                            {
                                case PaquetType.Information:
                                    var info = JsonConvert.DeserializeObject<Info>(ret);
                                    response.RequestId = info.RequestId;
                                    SendeMessage(response, listener);
                                    break;
                                case PaquetType.Ears:
                                    var ears = JsonConvert.DeserializeObject<Ears>(ret);
                                    response.RequestId = ears.RequestId;
                                    SendeMessage(response, listener);
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
                                    SendeMessage(response, listener);
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
                                    SendeMessage(response, listener);
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
                                    SendeMessage(response, listener);
                                    break;
                                case PaquetType.Wakeup:
                                    var wakeup = JsonConvert.DeserializeObject<Wakeup>(ret);
                                    state.State = StateType.Idle;
                                    response.RequestId = wakeup.RequestId;
                                    SendeMessage(response, listener);
                                    break;
                                case PaquetType.Sleep:
                                    var sleep = JsonConvert.DeserializeObject<Sleep>(ret);
                                    state.State = StateType.Asleep;
                                    response.RequestId = sleep.RequestId;
                                    SendeMessage(response, listener);
                                    break;
                                case PaquetType.Mode:
                                    var mode = JsonConvert.DeserializeObject<EventMode>(ret);
                                    response.RequestId = mode.RequestId;
                                    if (String.IsNullOrEmpty(response.RequestId))
                                    {
                                        response.Status = Status.Error;
                                        response.ErrorMessage = $"{nameof(Cancel.RequestId)} can't be empty or null";
                                        response.ErrorClass = "Mega error!";
                                    }
                                    else
                                    {
                                        if (eventId.ContainsKey(response.RequestId))
                                        {
                                            eventId[response.RequestId] = mode.Events;
                                        }
                                        else
                                        {
                                            eventId.Add(response.RequestId, mode.Events);
                                        }
                                    }
                                    SendeMessage(response, listener);
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
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception: {ex.Message}");
                        }
                    }
                }
            }

        }

        private static void SendeMessage(object objToSend, Socket listener)
        {
            var json = JsonConvert.SerializeObject(objToSend) + "\r\n";
            listener.Send(Encoding.UTF8.GetBytes(json));
            Console.WriteLine($"Sent response: {json}");
        }

    }
}
