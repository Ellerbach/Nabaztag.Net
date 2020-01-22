using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"mode","request_id":request_id,"mode":mode,"events":events}
    /// Emitter: services
    /// </summary>
    public class EventMode
    {
        /// <summary>
        /// Type is mode
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Mode; } }

        /// <summary>
        /// A request id, optional?
        /// </summary>
        [JsonProperty(PropertyName = "request_id", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestId { get; set; }

        /// <summary>
        /// The mode type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "mode")]
        public ModeType Mode { get; set; }

        /// <summary>
        /// The event type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "events")]
        public EventType[] Events { get; set; }
    }
}
