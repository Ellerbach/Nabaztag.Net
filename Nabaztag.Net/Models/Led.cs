using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// Enum for the Led on the rabbit
    /// </summary>
    public enum Led 
    {
        /// <summary>
        /// Nose
        /// </summary>
        [EnumMember(Value = "nose")]
        Nose,
        /// <summary>
        /// Right
        /// </summary>
        [EnumMember(Value = "right")]
        Right,
        /// <summary>
        /// Center
        /// </summary>
        [EnumMember(Value = "center")]
        Center,
        /// <summary>
        /// Left
        /// </summary>
        [EnumMember(Value = "left")]
        Left,
        /// <summary>
        /// Bottom
        /// </summary>
        [EnumMember(Value = "bottom")]
        Bottom 
    }
}
