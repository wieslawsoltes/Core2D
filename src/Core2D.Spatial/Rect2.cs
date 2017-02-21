// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Spatial
{
    public struct Rect2
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Width;
        public readonly double Height;

        public Rect2(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public static Rect2 FromPoints(double x1, double y1, double x2, double y2, double dx = 0.0, double dy = 0.0)
        {
            double x = (double)Math.Min(x1 + dx, x2 + dx);
            double y = (double)Math.Min(y1 + dy, y2 + dy);
            double width = Math.Abs(Math.Max(x1 + dx, x2 + dx) - x);
            double height = Math.Abs(Math.Max(y1 + dy, y2 + dy) - y);
            return new Rect2(x, y, width, height);
        }

        public static Rect2 FromPoints(Point2 tl, Point2 br, double dx = 0.0, double dy = 0.0)
        {
            return FromPoints(tl.X, tl.Y, br.X, br.Y, dx, dy);
        }

        public double Top
        {
            get { return Y; }
        }

        public double Left
        {
            get { return X; }
        }

        public double Bottom
        {
            get { return Y + Height; }
        }

        public double Right
        {
            get { return X + Width; }
        }

        public Point2 Center
        {
            get { return new Point2(X + Width / 2, Y + Height / 2); }
        }

        public Point2 TopLeft
        {
            get { return new Point2(X, Y); }
        }

        public Point2 BottomRight
        {
            get { return new Point2(X + Width, Y + Height); }
        }

        public bool Contains(Point2 point)
        {
            return ((point.X >= X)
                && (point.X - Width <= X)
                && (point.Y >= Y)
                && (point.Y - Height <= Y));
        }

        public bool Contains(double x, double y)
        {
            return ((x >= X)
                && (x - Width <= X)
                && (y >= Y)
                && (y - Height <= Y));
        }

        public bool IntersectsWith(Rect2 rect)
        {
            return (rect.Left <= Right)
                && (rect.Right >= Left)
                && (rect.Top <= Bottom)
                && (rect.Bottom >= Top);
        }
    }
}
