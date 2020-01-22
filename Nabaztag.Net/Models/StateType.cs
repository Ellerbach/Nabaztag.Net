using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
        /// The rabbit is playing a commmand
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
