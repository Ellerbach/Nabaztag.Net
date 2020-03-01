// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

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
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get; set; }
    }
}
