using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

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
        public string Nlu { get; set; }

        /// <summary>
        /// The timestamp
        /// </summary>
        [JsonProperty(PropertyName = "time")]
        public DateTime Time { get; set; }
    }
}
