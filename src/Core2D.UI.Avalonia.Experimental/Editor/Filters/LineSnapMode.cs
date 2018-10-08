// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Editor.Filters
{
    [Flags]
    public enum LineSnapMode
    {
        None = 0,
        Point = 1,
        Middle = 2,
        Intersection = 4,
        Horizontal = 8,
        Vertical = 16,
        Nearest = 32
    }
}
