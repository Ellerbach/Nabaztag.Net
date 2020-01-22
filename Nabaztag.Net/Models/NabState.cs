using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// A state type object
    /// Emitter : nabd
    /// </summary>
    public class NabState
    {
        /// <summary>
        /// Type is state
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.State; } }

        /// <summary>
        /// The state
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "state", Required = Required.Always)]
        public StateType State { get; set; }
    }
}
