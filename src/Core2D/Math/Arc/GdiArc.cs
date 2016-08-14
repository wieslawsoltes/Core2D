// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shapes;
using Core2D.Shapes.Interfaces;
using static System.Math;

namespace Core2D.Math.Arc
{
    /// <summary>
    /// Convert <see cref="IArc"/> coordinates to GDI arc coordinates.
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
        /// <returns></returns>
        public static GdiArc FromXArc(IArc arc)
        {
            return FromXArc(arc.Point1, arc.Point2, arc.Point3, arc.Point4);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        public static GdiArc FromXArc(XPoint p1, XPoint p2, XPoint p3, XPoint p4)
        {
            double x1 = p1.X;
            double y1 = p1.Y;
            double x2 = p2.X;
            double y2 = p2.Y;
            double x3 = p3.X;
            double y3 = p3.Y;
            double x4 = p4.X;
            double y4 = p4.Y;
            var rect = Rect2.Create(x1, y1, x2, y2);
            double cx = rect.X + rect.Width / 2.0;
            double cy = rect.Y + rect.Height / 2.0;
            double radiusX = cx - rect.X;
            double radiusY = cy - rect.Y;
            double startAngle = Atan2(y3 - cy, x3 - cx);
            double endAngle = Atan2(y4 - cy, x4 - cx);
            double sweepAngle = (endAngle - startAngle) * 180.0 / PI;

            if (sweepAngle < 0)
                sweepAngle += 360;

            startAngle *= 180.0 / PI;
            endAngle *= 180.0 / PI;

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
