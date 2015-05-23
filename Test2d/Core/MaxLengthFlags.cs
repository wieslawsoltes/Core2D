// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum MaxLengthFlags
    {
        /// <summary>
        /// 
        /// </summary>
        Disabled = 0,
        /// <summary>
        /// 
        /// </summary>
        Start = 1,
        /// <summary>
        /// 
        /// </summary>
        End = 2,
        /// <summary>
        /// 
        /// </summary>
        Vertical = 4,
        /// <summary>
        /// 
        /// </summary>
        Horizontal = 8,
        /// <summary>
        /// 
        /// </summary>
        All = 16
    }
}
