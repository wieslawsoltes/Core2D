// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// Group code: 70
    /// </summary>
    public enum DxfStyleFlags : int
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,
        /// <summary>
        /// 
        /// </summary>
        Shape = 1,
        /// <summary>
        /// 
        /// </summary>
        VerticalText = 4,
        /// <summary>
        /// 
        /// </summary>
        Xref = 16,
        /// <summary>
        /// 
        /// </summary>
        XrefSuccess = 32,
        /// <summary>
        /// 
        /// </summary>
        Referenced = 64
    }
}
