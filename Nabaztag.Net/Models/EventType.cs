// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// Event type
    /// </summary>    
    public enum EventType
    {
        /// <summary>
        /// Button
        /// </summary>   
        [EnumMember(Value = "button")]
        [JsonProperty(PropertyName = "button")]
        Button,
        /// <summary>
        /// Ears
        /// </summary>
        [EnumMember(Value = "ears")]
        [JsonProperty(PropertyName = "ears")]
        Ears,
        /// <summary>
        /// Asr
        /// </summary>
        [EnumMember(Value = "asr")]
        [JsonProperty(PropertyName = "asr")]
        Asr
    }
}
