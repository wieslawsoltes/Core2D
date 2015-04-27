// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    // Group code: 70
    [Flags]
    public enum DxfSplineFlags : int
    {
        ClosedSpline = 1,
        PeriodicSpline = 2,
        RationalSpline = 4,
        Planar = 8,
        Linear = 16
    }
}
