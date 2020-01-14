using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"audio":audio_list,"choreography":choreography}
    /// </summary>
    public class Sequence
    {
        [JsonProperty(PropertyName = "audio_list")]
        public string Audio_List { get; set; }
        public string Choreography { get; set; }
    }
}
