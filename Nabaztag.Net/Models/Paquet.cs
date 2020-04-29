// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// Core structure of a paquet
    /// </summary>
    public class Paquet
    {
        /// <summary>
        /// The type of paquet, determine the type of the element
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get; set; }

        /// <summary>
        /// A request id, optional
        /// </summary>
        [JsonProperty(PropertyName = "request_id", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestId { get; set; }
    }
}
