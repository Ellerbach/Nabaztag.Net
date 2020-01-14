using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"mode","request_id":request_id,"mode":mode,"events":events}
    /// Emmiter: services
    /// </summary>
    public class EventMode
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Mode; } }
        [JsonProperty(PropertyName = "request_id")]
        public string RequestId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "mode")]
        public ModeType Mode { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "events")]
        public EventType[] Events { get; set; }
    }
}
