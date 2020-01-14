using Nabaztag.Net.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nabaztag.Net
{
    public class Nabaztag
    {
        private const int DefaultTcpPortEmmit = 10543;
        private Dictionary<string, Response> _LastRequestId = new Dictionary<string, Response>();
        private TcpClient _TcpClient;

        // public event s
        public delegate void EarsEventHandler(object sender, EarsEvent ears);
        public event EarsEventHandler EarsEvent;
        public delegate void ButtonEventHandler(object sender, ButtonEvent buttonEvent);
        public event ButtonEventHandler ButtonEvent;
        public delegate void StateEventHandler(object sender, StateObject state);
        public event StateEventHandler StateEvent;

        public StateObject State { get; internal set; }

        public Nabaztag():this("localhost", DefaultTcpPortEmmit)
        {
        }

        public Nabaztag(string hostName, int tcpPort)
        {
            _TcpClient = new TcpClient(hostName, tcpPort);
            Task.Factory.StartNew(() => ProcessIncoming());

        }

        /// <summary>
        /// Wakeup the Nabaztag
        /// </summary>
        /// <param name="needRequestId">true if you want a confirmation, by default, yes</param>
        /// <param name="cancelAfterSeconds">Cancel waiting for the answer after the seconds defined. By default, it will wait indefintely</param>
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
        /// <param name="cancelAfterSeconds">Cancel waiting for the answer after the seconds defined. By default, it will wait indefintely</param>
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

        private bool SendMessage(string message)
        {
            if (!_TcpClient.Client.Connected)
                return false;
            message += "\r\n";
            var ret = _TcpClient.Client.Send(Encoding.UTF8.GetBytes(message));
            return (ret > 0);
        }

        private Response SendMessageProcessResponse(string toSend, Guid reqId, int cancelAfterSeconds)
        {
            var msgok = SendMessage(toSend);
            if (!msgok)
                return new Response() { Status = Status.Error, Class = "Sending message", Message = "Error sending message, check you have TCP/IP connection" };
            if (reqId == null)
                return new Response() { Status = Status.Ok };
            DateTime exp = DateTime.Now.AddSeconds(cancelAfterSeconds);
            bool isExpired = false;
            while ((_LastRequestId[reqId.ToString()] == null)
                || (isExpired))
            {
                isExpired = (exp > DateTime.Now) && (cancelAfterSeconds > 0);
            }
            if (isExpired)
            {
                _LastRequestId.Remove(reqId.ToString());
                Debug.WriteLine("Expiration");
                return new Response() { Status = Status.Expired };
            }
            Console.WriteLine($"Response received: {_LastRequestId[reqId.ToString()].Status}");
            return _LastRequestId[reqId.ToString()];
        }

        private void ProcessIncoming()
        {
            //Listen to tcp
            while (true)
            {
                var received = _TcpClient.Available;
                if (received > 0)
                {
                    byte[] buffer = new byte[received];
                    received = _TcpClient.GetStream().ReadAsync(buffer, 0, received).GetAwaiter().GetResult();
                    var res = Encoding.UTF8.GetString(buffer, 0, received); // text coming from TCP listener
                    try
                    {
                        var ret = JsonConvert.DeserializeObject<Paquet>(res);
                        switch (ret.Type)
                        {
                            case PaquetType.State:
                                State = JsonConvert.DeserializeObject<StateObject>(res);
                                StateEvent?.Invoke(this, State);
                                break;
                            case PaquetType.EarsEvent:
                                var earsEvent = JsonConvert.DeserializeObject<EarsEvent>(res);
                                EarsEvent?.Invoke(this, earsEvent);
                                break;
                            case PaquetType.ButtonEvent:
                                var buttonEvent = JsonConvert.DeserializeObject<ButtonEvent>(res);
                                ButtonEvent?.Invoke(this, buttonEvent);
                                break;
                            case PaquetType.Response:
                                var response = JsonConvert.DeserializeObject<Response>(res);
                                _LastRequestId[response.RequestId] = response;
                                break;
                            default:
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        ;
                    }
                }
            }
        }
    }
}
