// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    ///  Group code: 70
    /// </summary>
    public enum DxfBlockTypeFlags : int
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,
        /// <summary>
        /// 
        /// </summary>
        Anonymous = 1,
        /// <summary>
        /// 
        /// </summary>
        NonConstantAttributes = 2,
        /// <summary>
        /// 
        /// </summary>
        Xref = 4,
        /// <summary>
        /// 
        /// </summary>
        XrefOverlay = 8,
        /// <summary>
        /// 
        /// </summary>
        Dependant = 16,
        /// <summary>
        /// 
        /// </summary>
        Reference = 32,
        /// <summary>
        /// 
        /// </summary>
        ReferencesSuccess = 64
    }
}
