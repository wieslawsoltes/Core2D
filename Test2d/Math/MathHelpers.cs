// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public static class MathHelpers
    {
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
        /// <param name="line1Start"></param>
        /// <param name="line1End"></param>
        /// <param name="line2Start"></param>
        /// <param name="line2End"></param>
        /// <returns></returns>
        public static double AngleLineSegments(
            Point2 line1Start, Point2 line1End,
            Point2 line2Start, Point2 line2End)
        {
            double angle1 = Math.Atan2(line1Start.Y - line1End.Y, line1Start.X - line1End.X);
            double angle2 = Math.Atan2(line2Start.Y - line2End.Y, line2Start.X - line2End.X);
            double result = (angle2 - angle1) * 180.0 / Math.PI;
            if (result < 0)
                result += 360.0;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="onlySegment"></param>
        /// <returns></returns>
        public static IList<Point2> FindEllipseSegmentIntersections(
            Rect2 rect,
            Point2 p1, Point2 p2,
            bool onlySegment)
        {
            if ((rect.Width == 0) || (rect.Height == 0) || ((p1.X == p2.X) && (p1.Y == p2.Y)))
                return new Point2[] { };

            if (rect.Width < 0)
            {
                rect.X = rect.Right;
                rect.Width = -rect.Width;
            }

            if (rect.Height < 0)
            {
                rect.Y = rect.Bottom;
                rect.Height = -rect.Height;
            }

            double cx = rect.Left + rect.Width / 2.0;
            double cy = rect.Top + rect.Height / 2.0;

            rect.X -= cx;
            rect.Y -= cy;

            p1.X -= cx;
            p1.Y -= cy;
            p2.X -= cx;
            p2.Y -= cy;

            double a = rect.Width / 2.0;
            double b = rect.Height / 2.0;

            double A = (p2.X - p1.X) * (p2.X - p1.X) / a / a + (p2.Y - p1.Y) * (p2.Y - p1.Y) / b / b;
            double B = 2 * p1.X * (p2.X - p1.X) / a / a + 2 * p1.Y * (p2.Y - p1.Y) / b / b;
            double C = p1.X * p1.X / a / a + p1.Y * p1.Y / b / b - 1;

            var solutions = new List<double>();

            double discriminant = B * B - 4 * A * C;
            if (discriminant == 0)
            {
                solutions.Add(-B / 2 / A);
            }
            else if (discriminant > 0)
            {
                solutions.Add((-B + Math.Sqrt(discriminant)) / 2 / A);
                solutions.Add((-B - Math.Sqrt(discriminant)) / 2 / A);
            }

            var points = new List<Point2>();

            foreach (var t in solutions)
            {
                if (!onlySegment || ((t >= 0f) && (t <= 1f)))
                {
                    double x = p1.X + (p2.X - p1.X) * t + cx;
                    double y = p1.Y + (p2.Y - p1.Y) * t + cy;
                    points.Add(Point2.Create(x, y));
                }
            }

            return points;
        }
    }
}
