using Nabaztag.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabaztag.Server.Models
{
    public class EarsEventArguments
    {
        public Ear Ear { get; set; }
        public int Position { get; set; }
        public DateTime DateTimeEvent { get; set; }
    }
}
