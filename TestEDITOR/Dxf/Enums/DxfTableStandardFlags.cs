// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// Group code: 70
    /// </summary>
    public enum DxfTableStandardFlags : int
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,
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
