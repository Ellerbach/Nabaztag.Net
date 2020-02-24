using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        [EnumMember(Value = "info")]
        [JsonProperty(PropertyName = "info")]
        Information,
        /// <summary>
        /// Ears
        /// </summary>
        [EnumMember(Value = "ears")]
        [JsonProperty(PropertyName = "ears")]
        Ears,
        /// <summary>
        /// command
        /// </summary>
        [EnumMember(Value = "command")]
        [JsonProperty(PropertyName = "command")]
        Command,
        /// <summary>
        /// Message
        /// </summary>
        [EnumMember(Value = "message")]
        [JsonProperty(PropertyName = "message")]
        Message,
        /// <summary>
        /// Cancel
        /// </summary>
        [EnumMember(Value = "cancel")]
        [JsonProperty(PropertyName = "cancel")]
        Cancel,
        /// <summary>
        /// Wakeup
        /// </summary>
        [EnumMember(Value = "wakeup")]
        [JsonProperty(PropertyName = "wakeup")]
        Wakeup,
        /// <summary>
        /// Sleep
        /// </summary>
        [EnumMember(Value = "sleep")]
        [JsonProperty(PropertyName = "asleep")]
        Sleep,
        /// <summary>
        /// Mode
        /// </summary>
        [EnumMember(Value = "mode")]
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
