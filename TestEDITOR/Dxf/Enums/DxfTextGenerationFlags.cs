// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    /// <summary>
    /// Group code: 71, default = 0
    /// </summary>
    public enum DxfTextGenerationFlags : int
    {
        /// <summary>
        /// 
        /// </summary>
        Default = 0,
        /// <summary>
        /// 
        /// </summary>
        MirroredInX = 2,
        /// <summary>
        /// 
        /// </summary>
        MirroredInY = 4
    }
}
