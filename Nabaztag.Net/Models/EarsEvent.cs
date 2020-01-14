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
        [JsonProperty(PropertyName = "left")]
        public int? Left { get; set; }
        [JsonProperty(PropertyName = "right")]
        public int? Right { get; set; }
        [JsonProperty(PropertyName = "ear")]
        public int? Ear { get; set; }
    }
}
