// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// Status from the response
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// OK
        /// </summary>
        [JsonProperty(PropertyName = "ok")]
        [EnumMember(Value = "ok")]
        Ok,
        /// <summary>
        /// canceled
        /// </summary>
        [JsonProperty(PropertyName = "canceled")]
        [EnumMember(Value = "canceled")]
        Canceled,
        /// <summary>
        /// Expired
        /// </summary>
        [JsonProperty(PropertyName = "expired")]
        [EnumMember(Value = "expired")]
        Expired,
        /// <summary>
        /// Error
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        [EnumMember(Value = "error")]
        Error
    }
}
