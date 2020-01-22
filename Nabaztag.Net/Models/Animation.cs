using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"tempo":tempo, "colors":colors}
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Time in millisecond of the animation
        /// </summary>
        [JsonProperty(PropertyName = "tempo")]
        public int Tempo { get; set; }
        
        /// <summary>
        /// Color object representing a color of the one of the les
        /// </summary>
        [JsonProperty(PropertyName = "colors")]
        public Colors Colors { get; set; }
    }
}
