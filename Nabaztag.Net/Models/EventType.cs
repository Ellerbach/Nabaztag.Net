using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    public enum EventType
    {
        [JsonProperty(PropertyName = "button")]
        Button,
        [JsonProperty(PropertyName = "ears")]
        Ears
    }
}
