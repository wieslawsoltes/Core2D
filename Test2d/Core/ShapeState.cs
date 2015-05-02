// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    [Flags]
    public enum ShapeState
    {
        Default = 0,
        Visible = 1,
        Printable = 2,
        Locked = 4,
        Connector = 8,
        None = 16,
        Standalone = 32,
        Input = 64,
        Output = 128
    }
}
