using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Nabaztag.Net.Models
{
    /// <summary>
    /// The Ear enum
    /// </summary>
    public enum Ear 
    {
        /// <summary>
        /// Right
        /// </summary>
        [EnumMember(Value = "right")]
        Right,
        /// <summary>
        /// Left
        /// </summary>
        [EnumMember(Value = "left")]
        Left 
    }
}
