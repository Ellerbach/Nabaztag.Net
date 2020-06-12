using Nabaztag.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Nabaztag.Server.Models
{
    internal class EventNotification
    {
        public EventType[] EventType { get; set; }
        public string RequestId { get; set; }
        public ModeType Mode { get; set; }
    }
}
