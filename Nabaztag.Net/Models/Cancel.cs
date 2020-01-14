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
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Cancel; } }
        [JsonProperty(PropertyName = "request_id")]
        public string RequestId { get; set; }
    }
}
