// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Editor.Selection
{
    [Flags]
    public enum SelectionMode
    {
        None = 0,
        Point = 1,
        Shape = 2
    }
}
