// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    // Group code: 71, default = 0
    public enum DxfTextGenerationFlags : int
    {
        Default = 0,
        MirroredInX = 2,
        MirroredInY = 4
    }
}
