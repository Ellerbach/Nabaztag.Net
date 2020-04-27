// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"message","request_id":request_id,"signature":signature,"body":body,"cancelable":cancelable}
    /// Emitter: services
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The type is a message
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get { return PaquetType.Message; } }

        /// <summary>
        /// Expiration date, optional
        /// TODO: serialize as valid ISO 8601 dates
        /// </summary>
        [JsonProperty(PropertyName = "expiration", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// A request id, optional
        /// </summary>
        [JsonProperty(PropertyName = "request_id", NullValueHandling = NullValueHandling.Ignore)]
        public string RequestId { get; set; }

        /// <summary>
        /// A signature which is a sequence, optional
        /// </summary>
        [JsonProperty(PropertyName = "signature", NullValueHandling = NullValueHandling.Ignore)]
        public Sequence Signature { get; set; }

        /// <summary>
        /// A body which is a sequence
        /// </summary>
        [JsonProperty(PropertyName = "body", Required = Required.Always)]
        public Sequence[] Body { get; set; }

        /// <summary>
        /// If this is cancelable
        /// </summary>
        [JsonProperty(PropertyName = "cancelable", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Cancelable { get; set; }
    }
}
