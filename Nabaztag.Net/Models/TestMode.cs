using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// Lunch a test for ears or leds
    /// {"type":"test","test":"ears","request_id":"test"}
    /// {"type":"test","test":"leds","request_id":"test"}
    /// </summary>
    public class TestMode
    {
        /// <summary>
        /// Type is ears
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Test; } }

        /// <summary>
        /// A request id, optional
        /// </summary>
        [JsonProperty(PropertyName = "request_id", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestId { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "test", NullValueHandling = NullValueHandling.Ignore)]
        public TestType Test { get; set; }
    }
}
