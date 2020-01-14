using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    public enum PaquetType
    {
        [JsonProperty(PropertyName = "state")]
        State,
        [JsonProperty(PropertyName = "info")]
        Info,
        [JsonProperty(PropertyName = "ears")]
        Ears,
        [JsonProperty(PropertyName = "command")]
        Command,
        [JsonProperty(PropertyName = "message")]
        Message,
        [JsonProperty(PropertyName = "cancel")]
        Cancel,
        [JsonProperty(PropertyName = "wakeup")]
        Wakeup,
        [JsonProperty(PropertyName = "asleep")]
        Sleep,
        [JsonProperty(PropertyName = "mode")]
        Mode,
        [JsonProperty(PropertyName = "ears_event")]
        EarsEvent,
        [JsonProperty(PropertyName = "button_event")]
        ButtonEvent,
        [JsonProperty(PropertyName = "response")]
        Response
    }
}
