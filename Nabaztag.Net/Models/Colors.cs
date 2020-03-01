// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"left":color,"center":color,"right":color,"bottom":color,"nose":color}
    /// color can be one of the following: 
    ///  * a number from 0 to 15 for the original colors
    ///  * and HTML color with stating with #
    ///  * an symbolic color
    /// </summary>
    public class Colors
    {
        /// <summary>
        /// Color of the left led
        /// </summary>
        [JsonProperty(PropertyName = "left")]
        public string Left { get; set; }
        
        /// <summary>
        /// Color of the center led
        /// </summary>
        [JsonProperty(PropertyName = "center")]
        public string Center { get; set; }
        
        /// <summary>
        /// Color of the right led
        /// </summary>
        [JsonProperty(PropertyName = "right")]
        public string Right { get; set; }
        
        /// <summary>
        /// Color of the bottom led
        /// </summary>
        [JsonProperty(PropertyName = "bottom")]
        public string Bottom { get; set; }
        
        /// <summary>
        /// Color of the nose led
        /// </summary>
        [JsonProperty(PropertyName = "nose")]
        public string Nose { get; set; }
    }
}
