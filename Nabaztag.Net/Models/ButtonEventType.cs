// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// The type of event for button
    /// </summary>
    public enum ButtonEventType
    {
        /// <summary>
        /// Button pressed down
        /// </summary>
        [JsonProperty(PropertyName = "down")]
        Down,
        /// <summary>
        /// Button is up
        /// </summary>
        [JsonProperty(PropertyName = "up")]
        Up,
        /// <summary>
        /// Button is clicked
        /// </summary>
        [JsonProperty(PropertyName = "click")]
        Click,
        /// <summary>
        /// Button is double clicked
        /// </summary>
        [EnumMember(Value = "double_click")]
        [JsonProperty(PropertyName = "double_click")]
        DoubleClick,
        /// <summary>
        /// Button is click and hold
        /// </summary>
        [EnumMember(Value = "click_and_hold")]
        [JsonProperty(PropertyName = "click_and_hold")]
        ClickAndHold
    }
}
