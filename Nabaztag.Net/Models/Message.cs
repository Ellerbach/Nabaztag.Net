using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"message","request_id":request_id,"signature":signature,"body":body,"cancelable":cancelable}
    /// Emmiter: services
    /// </summary>
    public class Message
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Message; } }
        [JsonProperty(PropertyName = "expiration")]
        public DateTime? Expiration { get; set; }
        [JsonProperty(PropertyName = "request_id")]
        public string RequestId { get; set; }
        [JsonProperty(PropertyName = "signature")]
        public Sequence Signature { get; set; }
        [JsonProperty(PropertyName = "body")]
        public Sequence Body { get; set; }
        [JsonProperty(PropertyName = "cancelable")]
        public bool? Cancelable { get; set; }
    }
}
