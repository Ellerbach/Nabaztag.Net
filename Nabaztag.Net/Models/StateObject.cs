using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    public class StateObject
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.State; } }
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "state")]
        public StateType State { get; set; }
    }
}
