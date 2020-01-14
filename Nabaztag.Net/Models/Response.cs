using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"response","request_id":request_id,"status":"ok"}
    /// {"type":"response","request_id":request_id,"status":"canceled"}
    /// {"type":"response","request_id":request_id,"status":"expired"}
    /// {"type":"response","request_id":request_id,"status":"error","class":class,"message":message}
    /// Emmiter: nadb
    /// </summary>
    public class Response
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Response; } }
        [JsonProperty(PropertyName = "request_id", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "status")]
        public Status Status { get; set; }
        [JsonProperty(PropertyName = "class", NullValueHandling = NullValueHandling.Ignore)]
        public string Class { get; set; }
        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }
    }
}
