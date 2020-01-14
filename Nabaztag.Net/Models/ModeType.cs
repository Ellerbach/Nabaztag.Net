using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    public enum ModeType
    {
        [JsonProperty(PropertyName = "idle")]
        Idle,
        [JsonProperty(PropertyName = "interactive")]
        Interactive
    }
}
