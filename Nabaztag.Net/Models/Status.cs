// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

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
        Ok,
        /// <summary>
        /// canceled
        /// </summary>
        [JsonProperty(PropertyName = "canceled")]
        Canceled,
        /// <summary>
        /// Expired
        /// </summary>
        [JsonProperty(PropertyName = "expired")]
        Expired,
        /// <summary>
        /// Error
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        Error
    }
}
