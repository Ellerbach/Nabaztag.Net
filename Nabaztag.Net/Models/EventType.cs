using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
        [JsonProperty(PropertyName = "button")]
        Button,
        /// <summary>
        /// Ears
        /// </summary>
        [JsonProperty(PropertyName = "ears")]
        Ears
    }
}
