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
        [JsonProperty(PropertyName = "tempo")]
        public string Tempo { get; set; }
        [JsonProperty(PropertyName = "colors")]
        public Colors Colors { get; set; }
    }
}
