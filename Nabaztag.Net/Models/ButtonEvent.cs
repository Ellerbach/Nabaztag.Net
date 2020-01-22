using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"button_event","event":event}
    /// Emitter: nadb
    /// </summary>
    public class ButtonEvent
    {
        /// <summary>
        /// A cancel type
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.ButtonEvent; } }

        /// <summary>
        /// Event type
        /// </summary>
        [JsonProperty(PropertyName = "event", Required = Required.Always)]
        public ButtonEventType Event { get; set; }
    }
}
