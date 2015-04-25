// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    // Group code: 281
    public enum DxfViewRenderMode : byte
    {
        Optimized2D = 0,
        Wireframe = 1,
        HiddenLine = 2,
        FlatShaded= 3,
        GouraudShaded = 4,
        FlatShadedWithWireframe = 5,
        GouraudShadedWithWireframe = 6
    }
}
