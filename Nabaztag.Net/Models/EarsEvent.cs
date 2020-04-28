// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"ears_event","left":ear_left,"right":ear_right}
    /// </summary>

    public class EarsEvent
    {
        /// <summary>
        /// The left ear
        /// </summary>
        [JsonProperty(PropertyName = "left")]
        public int? Left { get; set; }

        /// <summary>
        /// The right ear
        /// </summary>
        [JsonProperty(PropertyName = "right")]
        public int? Right { get; set; }

        /// <summary>
        /// Ear moved
        /// </summary>
        [JsonProperty(PropertyName = "ear")]
        public Ear? Ear { get; set; }

        /// <summary>
        /// The timestamp
        /// </summary>
        [JsonProperty(PropertyName = "time")]
        public string Time { get; set; }
    }
}
