using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"command","request_id":request_id,"sequence":sequence,"expiration":expiration_date,"cancelable":cancelable}
    /// Emmiter: services
    /// </summary>
    public class Command
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Command; } }
        [JsonProperty(PropertyName = "request_id")]
        public string RequestId { get; set; }
        [JsonProperty(PropertyName = "sequence")]
        public Sequence Sequence { get; set; }
        [JsonProperty(PropertyName = "expiration_date")]
        public DateTime ExpirationDate { get; set; }
        [JsonProperty(PropertyName = "cancelable")]
        public bool Cancelable { get; set; }
    }
}
