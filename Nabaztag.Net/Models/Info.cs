using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"info","request_id":request_id,"info_id":info_id,"animation":animation}
    /// Emmiter: services
    /// </summary>
    public class Info
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public PaquetType Type { get { return PaquetType.Info; } }
        [JsonProperty(PropertyName = "request_id")]
        public string RequestId { get; set; }
        [JsonProperty(PropertyName = "info_id")]
        public string InfoId { get; set; }
        [JsonProperty(PropertyName = "animation")]
        public Animation Animation { get; set; }
    }
}
