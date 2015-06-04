// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// Group code: 71
    /// </summary>
    [Flags]
    public enum DxfViewMode : int
    {
        /// <summary>
        /// 
        /// </summary>
        Off = 0,
        /// <summary>
        /// 
        /// </summary>
        PerspectiveView = 1,
        /// <summary>
        /// 
        /// </summary>
        FrontClipping = 2,
        /// <summary>
        /// 
        /// </summary>
        BackClipping = 4,
        /// <summary>
        /// 
        /// </summary>
        UcsFollowMde = 8,
        /// <summary>
        /// 
        /// </summary>
        FrontClippingNotAtTheCamera = 16
    }
}
