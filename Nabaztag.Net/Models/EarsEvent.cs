using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"ears_event","left":ear_left,"right":ear_right}
    /// </summary>
    
    public class EarsEvent
    {
        /// <summary>
        /// The left ear
        /// </summary>
        [JsonProperty(PropertyName = "left")]
        public int? Left { get; set; }

        /// <summary>
        /// The right ear
        /// </summary>
        [JsonProperty(PropertyName = "right")]
        public int? Right { get; set; }

        /// <summary>
        /// Ear moved
        /// </summary>
        [JsonProperty(PropertyName = "ear")]
        public int? Ear { get; set; }
    }
}
