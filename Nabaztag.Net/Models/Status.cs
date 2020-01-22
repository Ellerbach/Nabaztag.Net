using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

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
