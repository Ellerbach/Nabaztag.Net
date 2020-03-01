// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
