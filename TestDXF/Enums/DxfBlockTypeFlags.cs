// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Dxf
{
    // Group code: 70
    public enum DxfBlockTypeFlags : int
    {
        Default = 0,
        Anonymous = 1,
        NonConstantAttributes = 2,
        Xref = 4,
        XrefOverlay = 8,
        Dependant = 16,
        Reference = 32,
        ReferencesSuccess = 64
    }
}
