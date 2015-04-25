// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    // Group code: 71
    [Flags]
    public enum DxfViewMode : int
    {
        Off = 0,
        PerspectiveView = 1,
        FrontClipping = 2,
        BackClipping = 4,
        UcsFollowMde = 8,
        FrontClippingNotAtTheCamera = 16
    }
}
