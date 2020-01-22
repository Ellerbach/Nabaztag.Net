using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"cancel","request_id":request_id}
    /// Emitter: services
    /// </summary>
    public class Cancel
    {
        /// <summary>
        /// A cancel type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Cancel; } }

        /// <summary>
        /// The request id to cancel
        /// </summary>
        [JsonProperty(PropertyName = "request_id", Required = Required.Always)]
        public string RequestId { get; set; }
    }
}
