using Nabaztag.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nabaztag.Server.Models
{
    public class Process
    {
        public PaquetType PaquetType { get; set; }
        public object ToProcess { get; set; }
    }
}
