// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"ears","request_id":request_id,"left":left_ear,"right":right_ear}
    /// Emitter: services
    /// </summary>
    public class Ears
    {
        /// <summary>
        /// Type is ears
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Ears; } }

        /// <summary>
        /// A request id, optional
        /// </summary>
        [JsonProperty(PropertyName = "request_id", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestId { get; set; }

        /// <summary>
        /// Left ear position, optional
        /// </summary>
        [JsonProperty(PropertyName = "left", NullValueHandling = NullValueHandling.Ignore)]
        public int Left { get; set; }

        /// <summary>
        /// Right ear position, optional
        /// </summary>
        [JsonProperty(PropertyName = "right", NullValueHandling = NullValueHandling.Ignore)]
        public int Right { get; set; }
    }
}
