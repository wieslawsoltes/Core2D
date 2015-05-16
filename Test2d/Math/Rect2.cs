// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public struct Rect2
    {
        /// <summary>
        /// 
        /// </summary>
        public double X;
        /// <summary>
        /// 
        /// </summary>
        public double Y;
        /// <summary>
        /// 
        /// </summary>
        public double Width;
        /// <summary>
        /// 
        /// </summary>
        public double Height;
        /// <summary>
        /// 
        /// </summary>
        public double Left { get { return X; } }
        /// <summary>
        /// 
        /// </summary>
        public double Right { get { return Width + X; } }
        /// <summary>
        /// 
        /// </summary>
        public double Top { get { return Y; } }
        /// <summary>
        /// 
        /// </summary>
        public double Bottom { get { return Y + Height; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Rect2(double x, double y, double width, double height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool Contains(Vector2 point)
        {
            return ((point.X >= X)
                && (point.X - Width <= X)
                && (point.Y >= Y)
                && (point.Y - Height <= Y));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool IntersectsWith(Rect2 rect)
        {
            return (rect.Left <= Right)
                && (rect.Right >= Left)
                && (rect.Top <= Bottom)
                && (rect.Bottom >= Top);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tl"></param>
        /// <param name="br"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
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
