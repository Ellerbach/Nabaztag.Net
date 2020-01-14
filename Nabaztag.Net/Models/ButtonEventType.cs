using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    public enum ButtonEventType
    {
        Down,
        Up,
        Click,
        [JsonProperty(PropertyName = "double_click")]
        DoubleClick,
        [JsonProperty(PropertyName = "click_and_hold")]
        ClickAndHold
    }
}
