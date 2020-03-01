// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// The mode type
    /// </summary>
    public enum ModeType
    {
        /// <summary>
        /// Idle
        /// </summary>
        [JsonProperty(PropertyName = "idle")]
        Idle,
        /// <summary>
        /// Interactive
        /// </summary>
        [JsonProperty(PropertyName = "interactive")]
        Interactive
    }
}
