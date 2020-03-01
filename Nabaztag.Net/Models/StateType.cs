// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// State of the rabbit
    /// Send by the nabd server
    /// Emitter: nabd
    /// </summary>
    public enum StateType
    {
        /// <summary>
        /// Rabbit is sleeping
        /// </summary>
        [JsonProperty(PropertyName = "asleep")]
        Asleep,
        /// <summary>
        /// Rabbit is awake and display info
        /// </summary>
        [JsonProperty(PropertyName = "idle")]
        Idle,
        /// <summary>
        /// The rabbit is in interactive mode
        /// </summary>
        [JsonProperty(PropertyName = "interactive")]
        Interactive,
        /// <summary>
        /// The rabbit is playing a command
        /// </summary>
        [JsonProperty(PropertyName = "playing")]
        Playing,
        /// <summary>
        /// The rabbit is recording
        /// </summary>
        [JsonProperty(PropertyName = "recording")]
        Recording
    }
}
