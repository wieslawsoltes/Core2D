// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    [Flags]
    public enum ShapeState
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,
        /// <summary>
        /// 
        /// </summary>
        Visible = 1,
        /// <summary>
        /// 
        /// </summary>
        Printable = 2,
        /// <summary>
        /// 
        /// </summary>
        Locked = 4,
        /// <summary>
        /// 
        /// </summary>
        Connector = 8,
        /// <summary>
        /// 
        /// </summary>
        None = 16,
        /// <summary>
        /// 
        /// </summary>
        Standalone = 32,
        /// <summary>
        /// 
        /// </summary>
        Input = 64,
        /// <summary>
        /// 
        /// </summary>
        Output = 128
    }
}
