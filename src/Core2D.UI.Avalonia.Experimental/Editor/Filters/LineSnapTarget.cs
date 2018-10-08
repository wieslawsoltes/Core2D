// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Editor.Filters
{
    [Flags]
    public enum LineSnapTarget
    {
        None = 0,
        Guides = 1,
        Shapes = 2
    }
}
