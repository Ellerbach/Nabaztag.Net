// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"command","request_id":request_id,"sequence":sequence,"expiration":expiration_date,"cancelable":cancelable}
    /// Emitter: services
    /// </summary>
    public class Command
    {
        /// <summary>
        /// Type is a command
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Command; } }

        /// <summary>
        /// A request id, optional
        /// </summary>
        [JsonProperty(PropertyName = "request_id", NullValueHandling = NullValueHandling.Ignore)]       
        public string RequestId { get; set; }

        /// <summary>
        /// A sequence
        /// </summary>
        [JsonProperty(PropertyName = "sequence", Required = Required.Always)]
        public Sequence Sequence { get; set; }

        /// <summary>
        /// Expiration date, optional
        /// TODO: serialize as valid ISO 8601 dates
        /// </summary>
        [JsonProperty(PropertyName = "expiration", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// If the sequence is cancelable, optional
        /// </summary>
        [JsonProperty(PropertyName = "cancelable", NullValueHandling = NullValueHandling.Ignore)]
        public bool Cancelable { get; set; }
    }
}
