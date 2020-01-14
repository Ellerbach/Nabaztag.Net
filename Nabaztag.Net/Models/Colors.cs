using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"left":color,"center":color,"right":color,"bottom":color,"nose":color}
    /// color = TBD :-(
    /// </summary>
    public class Colors
    {
        [JsonProperty(PropertyName = "left")]
        public string Left { get; set; }
        [JsonProperty(PropertyName = "center")]
        public string Center { get; set; }
        [JsonProperty(PropertyName = "right")]
        public string Right { get; set; }
        [JsonProperty(PropertyName = "bottom")]
        public string Bottom { get; set; }
        [JsonProperty(PropertyName = "nose")]
        public string Nose { get; set; }
    }
}
