using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// Core structure of a paquet
    /// </summary>
    public class Paquet
    {
        /// <summary>
        /// The type of paquet, determine the type of the element
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public PaquetType Type { get; set; }
    }
}
