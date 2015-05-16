// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Test2d;

namespace Test
{
    /// <summary>
    /// 
    /// </summary>
    internal struct WpfArc
    {
        /// <summary>
        /// 
        /// </summary>
        public Point P1;

        /// <summary>
        /// 
        /// </summary>
        public Point P2;

        /// <summary>
        /// 
        /// </summary>
        public Point P3;

        /// <summary>
        /// 
        /// </summary>
        public Point P4;

        /// <summary>
        /// 
        /// </summary>
        public Rect Rect;

        /// <summary>
        /// 
        /// </summary>
        public Point Center;

        /// <summary>
        /// 
        /// </summary>
        public Point Start;

        /// <summary>
        /// 
        /// </summary>
        public Point End;

        /// <summary>
        /// 
        /// </summary>
        public Size Radius;

        /// <summary>
        /// 
        /// </summary>
        public bool IsLargeArc;

        /// <summary>
        /// 
        /// </summary>
        public double Angle;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static WpfArc FromXArc(XArc arc, double dx, double dy)
        {
            var p1 = new Point(arc.Point1.X + dx, arc.Point1.Y + dy);
            var p2 = new Point(arc.Point2.X + dx, arc.Point2.Y + dy);
            var p3 = new Point(arc.Point3.X + dx, arc.Point3.Y + dy);
            var p4 = new Point(arc.Point4.X + dx, arc.Point4.Y + dy);
            var rect = new Rect(p1, p2);
            var center = new Point(rect.X + rect.Width / 2.0, rect.Y + rect.Height / 2.0);
            double offsetX = center.X - rect.X;
            double offsetY = center.Y - rect.Y;

            double minLenght = Math.Max(offsetX, offsetY);

            double length1 = Distance(center, p3);
            double p3x = p3.X + (p3.X - center.X) / length1 * minLenght;
            double p3y = p3.Y + (p3.Y - center.Y) / length1 * minLenght;

            double length2 = Distance(center, p4);
            double p4x = p4.X + (p4.X - center.X) / length2 * minLenght;
            double p4y = p4.Y + (p4.Y - center.Y) / length2 * minLenght;

            p3.X = p3x;
            p3.Y = p3y;
            p4.X = p4x;
            p4.Y = p4y;

            var p3i = FindEllipseSegmentIntersections(rect, center, p3, true);
            var p4i = FindEllipseSegmentIntersections(rect, center, p4, true);
            Point start;
            Point end;

            if (p3i.Count == 1)
                start = p3i.FirstOrDefault();
            else
                start = new Point(p3.X, p3.Y);

            if (p4i.Count == 1)
                end = p4i.FirstOrDefault();
            else
                end = new Point(p4.X, p4.Y);

            double angle = AngleLineSegments(center, start, center, end);
            bool isLargeArc = angle > 180.0;

            double helperLenght = 60.0;

            double lengthStart = Distance(center, start);
            double p3hx = start.X + (start.X - center.X) / lengthStart * helperLenght;
            double p3hy = start.Y + (start.Y - center.Y) / lengthStart * helperLenght;

            double lengthEnd = Distance(center, end);
            double p4hx = end.X + (end.X - center.X) / lengthEnd * helperLenght;
            double p4hy = end.Y + (end.Y - center.Y) / lengthEnd * helperLenght;

            p3.X = p3hx;
            p3.Y = p3hy;
            p4.X = p4hx;
            p4.Y = p4hy;

            return new WpfArc()
            {
                P1 = p1,
                P2 = p2,
                P3 = p3,
                P4 = p4,
                Rect = rect,
                Center = center,
                Start = start,
                End = end,
                Radius = new Size(offsetX, offsetY),
                IsLargeArc = isLargeArc,
                Angle = angle
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line1Start"></param>
        /// <param name="line1End"></param>
        /// <param name="line2Start"></param>
        /// <param name="line2End"></param>
        /// <returns></returns>
        public static double AngleLineSegments(Point line1Start, Point line1End, Point line2Start, Point line2End)
        {
            double angle1 = Math.Atan2(line1Start.Y - line1End.Y, line1Start.X - line1End.X);
            double angle2 = Math.Atan2(line2Start.Y - line2End.Y, line2Start.X - line2End.X);
            double result = (angle2 - angle1) * 180 / Math.PI;
            if (result < 0)
                result += 360;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static double Distance(Point p1, Point p2)
        {
            double dx = p1.X - p2.X;
            double dy = p1.Y - p2.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="onlySegment"></param>
        /// <returns></returns>
        public static IList<Point> FindEllipseSegmentIntersections(Rect rect, Point p1, Point p2, bool onlySegment)
        {
            if ((rect.Width == 0) || (rect.Height == 0) || ((p1.X == p2.X) && (p1.Y == p2.Y)))
                return new Point[] { };

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

            var points = new List<Point>();

            foreach (var t in solutions)
            {
                if (!onlySegment || ((t >= 0f) && (t <= 1f)))
                {
                    double x = p1.X + (p2.X - p1.X) * t + cx;
                    double y = p1.Y + (p2.Y - p1.Y) * t + cy;
                    points.Add(new Point(x, y));
                }
            }

            return points;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointToRotate"></param>
        /// <param name="centerPoint"></param>
        /// <param name="angleInDegrees"></param>
        /// <returns></returns>
        public static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180.0);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X = (cosTheta * (pointToRotate.X - centerPoint.X) - sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y = (sinTheta * (pointToRotate.X - centerPoint.X) + cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
    }
}
