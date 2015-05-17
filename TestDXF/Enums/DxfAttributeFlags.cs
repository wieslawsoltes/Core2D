// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// Group code: 70
    /// </summary>
    public enum DxfAttributeFlags : int
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,
        /// <summary>
        /// 
        /// </summary>
        Invisible = 1,
        /// <summary>
        /// 
        /// </summary>
        Constant = 2,
        /// <summary>
        /// 
        /// </summary>
        Verification = 4,
        /// <summary>
        /// 
        /// </summary>
        Preset = 8
    }
}
