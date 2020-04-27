using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// Type of tests: ers or leds
    /// </summary>
   public enum TestType
    {
        /// <summary>
        /// The test type for ears
        /// </summary>
        [EnumMember(Value = "ears")]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "ears")]
        Ears,
        /// <summary>
        /// The test type for leds
        /// </summary>
        [EnumMember(Value = "leds")]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "leds")]
        Leds,
    }
}
