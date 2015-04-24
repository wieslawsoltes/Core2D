// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    // Group code: 70
    public enum DxfAttributeFlags : int
    {
        Default = 0,
        Invisible = 1,
        Constant = 2,
        Verification = 4,
        Preset = 8
    }
}
