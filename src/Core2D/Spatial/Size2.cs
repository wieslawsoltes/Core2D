// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

using System;

namespace Core2D.Spatial;

public struct Size2
{
    public readonly double Width;
    public readonly double Height;

    public Size2(double width, double height)
    {
        Width = width;
        Height = height;
    }

    public void Deconstruct(out double width, out double height)
    {
        width = Width;
        height = Height;
    }
}