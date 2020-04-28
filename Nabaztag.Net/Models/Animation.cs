// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"tempo":tempo, "colors":colors}
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Time in millisecond of the animation
        /// </summary>
        [JsonProperty(PropertyName = "tempo")]
        public int Tempo { get; set; }
        
        /// <summary>
        /// Color object representing a color of the one of the les
        /// </summary>
        [JsonProperty(PropertyName = "colors")]
        public Colors[] Colors { get; set; }
    }
}
