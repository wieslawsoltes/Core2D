// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public struct Point2
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
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Point2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public double Distance(Point2 point)
        {
            double dx = this.X - point.X;
            double dy = this.Y - point.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double Distance(double x, double y)
        {
            double dx = this.X - x;
            double dy = this.Y - y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="centerPoint"></param>
        /// <param name="angleInDegrees"></param>
        /// <returns></returns>
        public Point2 RotateAt(
            Point2 centerPoint,
            double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180.0);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point2
            {
                X = (cosTheta * (this.X - centerPoint.X) - sinTheta * (this.Y - centerPoint.Y) + centerPoint.X),
                Y = (sinTheta * (this.X - centerPoint.X) + cosTheta * (this.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Point2 Create(double x, double y, double dx = 0.0, double dy = 0.0)
        {
            return new Point2(x + dx, y + dy);
        }
    }
}
