// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public struct GdiArc
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
        public double RadiusX;

        /// <summary>
        /// 
        /// </summary>
        public double RadiusY;

        /// <summary>
        /// 
        /// </summary>
        public double StartAngle;

        /// <summary>
        /// 
        /// </summary>
        public double EndAngle;

        /// <summary>
        /// 
        /// </summary>
        public double SweepAngle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static GdiArc FromXArc(XArc arc, double dx, double dy)
        {
            return FromXArc(arc.Point1, arc.Point2, arc.Point3, arc.Point4, dx, dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static GdiArc FromXArc(XPoint p1, XPoint p2, XPoint p3, XPoint p4, double dx, double dy)
        {
            double x1 = p1.X + dx;
            double y1 = p1.Y + dy;
            double x2 = p2.X + dx;
            double y2 = p2.Y + dy;
            double x3 = p3.X + dx;
            double y3 = p3.Y + dy;
            double x4 = p4.X + dx;
            double y4 = p4.Y + dy;
            var rect = Rect2.Create(x1, y1, x2, y2, dx, dy);
            double cx = rect.X + rect.Width / 2.0;
            double cy = rect.Y + rect.Height / 2.0;
            double radiusX = cx - rect.X;
            double radiusY = cy - rect.Y;
            double startAngle = Math.Atan2(y3 - cy, x3 - cx);
            double endAngle = Math.Atan2(y4 - cy, x4 - cx);
            double sweepAngle = (endAngle - startAngle) * 180.0 / Math.PI;

            if (sweepAngle < 0)
                sweepAngle += 360;

            startAngle *= 180.0 / Math.PI;
            endAngle *= 180.0 / Math.PI;

            return new GdiArc
            {
                X = rect.X,
                Y = rect.Y,
                Width = rect.Width,
                Height = rect.Height,
                RadiusX = radiusX,
                RadiusY = radiusY,
                StartAngle = startAngle,
                EndAngle = endAngle,
                SweepAngle = sweepAngle
            };
        }
    }
}
