using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"ears","request_id":request_id,"left":left_ear,"right":right_ear}
    /// Emmiter: services
    /// </summary>
    public class Ears
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Ears; } }
        [JsonProperty(PropertyName = "request_id")]
        public string RequestId { get; set; }
        [JsonProperty(PropertyName = "left")]
        public int Left { get; set; }
        [JsonProperty(PropertyName = "right")]
        public int Right { get; set; }
    }
}
