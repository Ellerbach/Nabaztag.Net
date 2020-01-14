using System;
using System.Collections.Generic;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// {"type":"button_event","event":event}
    /// Emmiter: nadb
    /// </summary>
    public class ButtonEvent
    {
        public ButtonEventType Event { get; set; }
    }
}
