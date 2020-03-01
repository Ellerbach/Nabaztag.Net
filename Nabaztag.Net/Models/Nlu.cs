// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// Class for Asr events representing one intent
    /// {'intent': intent}
    /// </summary>
    public class Nlu
    {
        /// <summary>
        /// Intent
        /// </summary>
        [JsonProperty(PropertyName = "intent")]
        public string Intent { get; set; }
    }
}
