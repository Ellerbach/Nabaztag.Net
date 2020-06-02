using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// The statistics about the Nabaztag hardware and softare
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// Packet type if info
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get; set; }

        /// <summary>
        /// Request id, optional
        /// </summary>
        [JsonProperty(PropertyName = "request_id", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestId { get; set; }

        /// <summary>
        /// The hardware detail
        /// </summary>
        [JsonProperty(PropertyName = "hardware", NullValueHandling = NullValueHandling.Ignore)]
        public Hardware Hardware { get; set; }

        /// <summary>
        /// The state
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "state", NullValueHandling = NullValueHandling.Ignore)]
        public StateType State { get; set; }

        /// <summary>
        /// Uptime
        /// </summary>
        [JsonProperty(PropertyName = "uptime", NullValueHandling = NullValueHandling.Ignore)]
        public int Uptime { get; set; }

        /// <summary>
        /// Connections
        /// </summary>
        [JsonProperty(PropertyName = "connections", NullValueHandling = NullValueHandling.Ignore)]
        public int Connections { get; set; }

    }
}
