// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// 
    /// </summary>
    public enum DxfProxyCapabilitiesFlags : int
    {
        /// <summary>
        /// 
        /// </summary>
        NoOperationsAllowed = 0,
        /// <summary>
        /// 
        /// </summary>
        EraseAllowed = 1,
        /// <summary>
        /// 
        /// </summary>
        TransformAllowed = 2,
        /// <summary>
        /// 
        /// </summary>
        ColorChangeAllowed = 4,
        /// <summary>
        /// 
        /// </summary>
        LayerChangeAllowed = 8,
        /// <summary>
        /// 
        /// </summary>
        LinetypeChangeAllowed = 16,
        /// <summary>
        /// 
        /// </summary>
        LinetypeScaleChangeAllowed = 32,
        /// <summary>
        /// 
        /// </summary>
        VisibilityChangeAllowed = 64,
        /// <summary>
        /// 
        /// </summary>
        AllOperationsExceptCloningAllowed = 127,
        /// <summary>
        /// 
        /// </summary>
        CloningAllowed = 128,
        /// <summary>
        /// 
        /// </summary>
        AllOperationsAllowed = 255,
        /// <summary>
        /// 
        /// </summary>
        R13FormatProxy = 32768
    }
}
