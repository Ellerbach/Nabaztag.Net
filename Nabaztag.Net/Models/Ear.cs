﻿// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.Serialization;

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
