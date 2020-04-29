// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// The hardware details
    /// </summary>
    public class Hardware
    {
        /// <summary>
        /// The model
        /// </summary>
        [JsonProperty(PropertyName = "model")]
        public string Model { get; set; }
        /// <summary>
        /// The sound card type
        /// </summary>
        [JsonProperty(PropertyName = "sound_card")]
        public string SoundCard { get; set; }
        /// <summary>
        /// The sound input
        /// </summary>
        [JsonProperty(PropertyName = "sound_input")]
        public bool SoundInput { get; set; }
        /// <summary>
        /// Is RFID installed
        /// </summary>
        [JsonProperty(PropertyName = "rfid")]
        public bool IsRfid { get; set; }
        /// <summary>
        /// The status of the left hear
        /// </summary>
        [JsonProperty(PropertyName = "left_ear_status")]
        public string LeftEarStatus { get; set; }
        /// <summary>
        /// The status of the right hear
        /// </summary>
        [JsonProperty(PropertyName = "right_ear_status")]
        public string RrightEarStatus { get; set; }
    }

}