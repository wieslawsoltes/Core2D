// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// Group code: 70
    /// </summary>
    public enum DxfLayerStandardFlags : int
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,
        /// <summary>
        /// 
        /// </summary>
        Frozen = 1,
        /// <summary>
        /// 
        /// </summary>
        FrozenByDefault = 2,
        /// <summary>
        /// 
        /// </summary>
        Locked = 4,
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
        References = 64
    }
}
