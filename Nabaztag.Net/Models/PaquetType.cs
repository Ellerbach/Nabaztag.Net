using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// Paquet type
    /// </summary>
    public enum PaquetType
    {
        /// <summary>
        /// State
        /// </summary>
        [JsonProperty(PropertyName = "state")]
        State,
        /// <summary>
        /// Info
        /// </summary>
        [JsonProperty(PropertyName = "info")]
        Information,
        /// <summary>
        /// Ears
        /// </summary>
        [JsonProperty(PropertyName = "ears")]
        Ears,
        /// <summary>
        /// command
        /// </summary>
        [JsonProperty(PropertyName = "command")]
        Command,
        /// <summary>
        /// Message
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        Message,
        /// <summary>
        /// Cancel
        /// </summary>
        [JsonProperty(PropertyName = "cancel")]
        Cancel,
        /// <summary>
        /// Wakeup
        /// </summary>
        [JsonProperty(PropertyName = "wakeup")]
        Wakeup,
        /// <summary>
        /// Sleep
        /// </summary>
        [JsonProperty(PropertyName = "asleep")]
        Sleep,
        /// <summary>
        /// Mode
        /// </summary>
        [JsonProperty(PropertyName = "mode")]
        Mode,
        /// <summary>
        /// Ears event
        /// </summary>
        [JsonProperty(PropertyName = "ears_event")]
        EarsEvent,
        /// <summary>
        /// Button event
        /// </summary>
        [JsonProperty(PropertyName = "button_event")]
        ButtonEvent,
        /// <summary>
        /// Response
        /// </summary>
        [JsonProperty(PropertyName = "response")]
        Response,
        /// <summary>
        /// Asr event
        /// </summary>
        [JsonProperty(PropertyName = "asr_event")]
        AsrEvent
    }
}
