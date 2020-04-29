using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// The statistics about the Nabaztag hardware and softare
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// The hardware detail
        /// </summary>
        public Hardware Hardware { get; set; }
        
        /// <summary>
        /// The state
        /// </summary>
        public StateType State { get; set; }

        /// <summary>
        /// Uptime
        /// </summary>
        public int Uptime { get; set; }

        /// <summary>
        /// Connections
        /// </summary>
        public int Connections { get; set; }

    }
}
