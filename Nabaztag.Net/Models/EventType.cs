// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// Event type
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// Button
        /// </summary>
        [JsonProperty(PropertyName = "button")]
        Button,
        /// <summary>
        /// Ears
        /// </summary>
        [JsonProperty(PropertyName = "ears")]
        Ears,
        /// <summary>
        /// Asr
        /// </summary>
        [JsonProperty(PropertyName = "asr")]
        Asr
    }
}
