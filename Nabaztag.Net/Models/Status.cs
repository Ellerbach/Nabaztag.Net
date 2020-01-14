using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    public enum Status
    {
        [JsonProperty(PropertyName = "ok")]
        Ok,
        [JsonProperty(PropertyName = "cancel")]
        Cancel,
        [JsonProperty(PropertyName = "expired")]
        Expired,
        [JsonProperty(PropertyName = "error")]
        Error
    }
}
