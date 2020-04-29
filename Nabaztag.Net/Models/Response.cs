// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"response","request_id":request_id,"status":"ok"}
    /// {"type":"response","request_id":request_id,"status":"canceled"}
    /// {"type":"response","request_id":request_id,"status":"expired"}
    /// {"type":"response","request_id":request_id,"status":"error","class":class,"message":message}
    /// Emitter: nadb
    /// </summary>
    public class Response
    {
        /// <summary>
        /// Type is response
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Response; } }

        /// <summary>
        /// A request id, optional
        /// </summary>
        [JsonProperty(PropertyName = "request_id", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestId { get; set; }

        /// <summary>
        /// The status
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "status")]
        public Status? Status { get; set; }

        /// <summary>
        /// A class of error in case of error
        /// </summary>
        [JsonProperty(PropertyName = "class", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorClass { get; set; }

        /// <summary>
        /// The error message in case of error
        /// </summary>
        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The state
        /// </summary>
        [JsonProperty(PropertyName = "state", NullValueHandling = NullValueHandling.Ignore)]
        public StateType State { get; set; }

        /// <summary>
        /// Uptime
        /// </summary>
        [JsonProperty(PropertyName = "uptime", NullValueHandling = NullValueHandling.Ignore)]
        public int Uptime { get; set; }

        /// <summary>
        /// Connections
        /// </summary>
        [JsonProperty(PropertyName = "connections", NullValueHandling = NullValueHandling.Ignore)]
        public int Connections { get; set; }
        /// <summary>
        /// Connections
        /// </summary>
        [JsonProperty(PropertyName = "hardware", NullValueHandling = NullValueHandling.Ignore)]
        public Hardware Hardware { get; set; }
    }
}
