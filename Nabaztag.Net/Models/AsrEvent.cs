// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using System;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// "asr", {"type": "asr_event", "nlu": response, "time": now}
    /// Emitter: nabd
    /// </summary>
    public class AsrEvent
    {
        /// <summary>
        /// What has been understand by the NLU
        /// </summary>
        [JsonProperty(PropertyName = "nlu")]
        public Nlu Nlu { get; set; }

        /// <summary>
        /// The timestamp
        /// </summary>
        [JsonProperty(PropertyName = "time")]
        public string Time { get; set; }
    }
}
