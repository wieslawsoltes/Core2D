// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    public struct Rect2
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;
        public double Left { get { return X; } }
        public double Right { get { return Width + X; } }
        public double Top { get { return Y; } }
        public double Bottom { get { return Y + Height; } }

        public Rect2(double x, double y, double width, double height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public bool Contains(Vector2 point)
        {
            return ((point.X >= X)
                && (point.X - Width <= X)
                && (point.Y >= Y)
                && (point.Y - Height <= Y));
        }

        public bool IntersectsWith(Rect2 rect)
        {
            return (rect.Left <= Right)
                && (rect.Right >= Left)
                && (rect.Top <= Bottom)
                && (rect.Bottom >= Top);
        }

        public static Rect2 Create(XPoint tl, XPoint br, double dx = 0.0, double dy = 0.0)
        {
            double tlx = Math.Min(tl.X, br.X);
            double tly = Math.Min(tl.Y, br.Y);
            double brx = Math.Max(tl.X, br.X);
            double bry = Math.Max(tl.Y, br.Y);
            double x = tlx + dx;
            double y = tly + dy;
            double width = (brx + dx) - x;
            double height = (bry + dy) - y;
            return new Rect2(x, y, width, height);
        }
    }
}
