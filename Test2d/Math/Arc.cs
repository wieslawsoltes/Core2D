// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public struct Arc
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
        /// Converts an angle in decimal degress to radians.
        /// </summary>
        /// <param name="angleInDegrees">Angle in decimal degrees.</param>
        /// <returns>Angle in radians.</returns>
        public static double DegreesToRadians(double angleInDegrees)
        {
            return angleInDegrees * (Math.PI / 180.0);
        }

        /// <summary>
        /// Converts an angle in radians to decimal degress.
        /// </summary>
        /// <param name="angleInRadians">Angle in radians</param>
        /// <returns>Angle in decimal degrees.</returns>
        public static double RadiansToDegrees(double angleInRadians)
        {
            return angleInRadians * (180.0 / Math.PI);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static Arc FromXArc(XArc arc, double dx, double dy)
        {
            double x1 = arc.Point1.X + dx;
            double y1 = arc.Point1.Y + dy;
            double x2 = arc.Point2.X + dx;
            double y2 = arc.Point2.Y + dy;
            double x3 = arc.Point3.X + dx;
            double y3 = arc.Point3.Y + dy;
            double x4 = arc.Point4.X + dx;
            double y4 = arc.Point4.Y + dy;
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

            return new Arc
            {
                X = rect.X,
                Y = rect.Y,
                Width = rect.Width,
                Height = rect.Height,
                RadiusX = radiusX,
                RadiusY = radiusY,
                StartAngle = startAngle,
                EndAngle =  endAngle,
                SweepAngle = sweepAngle
            };
        }
    }
}
