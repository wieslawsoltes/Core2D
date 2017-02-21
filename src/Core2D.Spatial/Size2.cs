// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Spatial
{
    public struct Size2
    {
        public readonly double Width;
        public readonly double Height;

        public Size2(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}
