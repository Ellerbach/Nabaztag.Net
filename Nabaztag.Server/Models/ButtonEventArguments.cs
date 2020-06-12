using Nabaztag.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabaztag.Server.Models
{
    public class ButtonEventArguments
    {
        public ButtonEventType ButtonEventType { get; set; }
        public DateTime DateTimeEvent { get; set; }
    }
}
