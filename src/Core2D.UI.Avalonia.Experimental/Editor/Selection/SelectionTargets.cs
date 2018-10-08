// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Editor.Selection
{
    [Flags]
    public enum SelectionTargets
    {
        None = 0,
        Shapes = 1,
        Guides = 2
    }
}
