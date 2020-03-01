// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

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
