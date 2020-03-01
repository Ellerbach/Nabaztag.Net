// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"info","request_id":request_id,"info_id":info_id,"animation":animation}
    /// Emitter: services
    /// </summary>
    public class Info
    {
        /// <summary>
        /// Packet type if info
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PaquetType Type { get { return PaquetType.Information; } }
        
        /// <summary>
        /// Request id, optional
        /// </summary>
        [JsonProperty(PropertyName = "request_id", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestId { get; set; }
        
        /// <summary>
        /// The information Id, mandatory
        /// </summary>
        [JsonProperty(PropertyName = "info_id", Required = Required.Always)]
        public string InfoId { get; set; }
        
        /// <summary>
        /// An animation, optional
        /// </summary>
        [JsonProperty(PropertyName = "animation", NullValueHandling = NullValueHandling.Ignore)]
        public Animation Animation { get; set; }
    }
}
