using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
