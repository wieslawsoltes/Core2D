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
    public struct WpfArc
    {
        /// <summary>
        /// 
        /// </summary>
        public Point2 P1;

        /// <summary>
        /// 
        /// </summary>
        public Point2 P2;

        /// <summary>
        /// 
        /// </summary>
        public Point2 P3;

        /// <summary>
        /// 
        /// </summary>
        public Point2 P4;

        /// <summary>
        /// 
        /// </summary>
        public Rect2 Rect;

        /// <summary>
        /// 
        /// </summary>
        public Point2 Center;

        /// <summary>
        /// 
        /// </summary>
        public Point2 Start;

        /// <summary>
        /// 
        /// </summary>
        public Point2 End;

        /// <summary>
        /// 
        /// </summary>
        public Size2 Radius;

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
            var p1 = Point2.Create(arc.Point1.X + dx, arc.Point1.Y + dy);
            var p2 = Point2.Create(arc.Point2.X + dx, arc.Point2.Y + dy);
            var p3 = Point2.Create(arc.Point3.X + dx, arc.Point3.Y + dy);
            var p4 = Point2.Create(arc.Point4.X + dx, arc.Point4.Y + dy);
            var rect = Rect2.Create(p1, p2);
            var center = Point2.Create(rect.X + rect.Width / 2.0, rect.Y + rect.Height / 2.0);
            double offsetX = center.X - rect.X;
            double offsetY = center.Y - rect.Y;

            double minLenght = Math.Max(offsetX, offsetY);

            double length1 = center.Distance(p3);
            double p3x = p3.X + (p3.X - center.X) / length1 * minLenght;
            double p3y = p3.Y + (p3.Y - center.Y) / length1 * minLenght;

            double length2 = center.Distance(p4);
            double p4x = p4.X + (p4.X - center.X) / length2 * minLenght;
            double p4y = p4.Y + (p4.Y - center.Y) / length2 * minLenght;

            p3.X = p3x;
            p3.Y = p3y;
            p4.X = p4x;
            p4.Y = p4y;

            var p3i = MathHelpers.FindEllipseSegmentIntersections(rect, center, p3, true);
            var p4i = MathHelpers.FindEllipseSegmentIntersections(rect, center, p4, true);
            Point2 start;
            Point2 end;

            if (p3i.Count == 1)
                start = p3i.FirstOrDefault();
            else
                start = Point2.Create(p3.X, p3.Y);

            if (p4i.Count == 1)
                end = p4i.FirstOrDefault();
            else
                end = Point2.Create(p4.X, p4.Y);

            double angle = MathHelpers.AngleLineSegments(center, start, center, end);
            bool isLargeArc = angle > 180.0;

            double helperLenght = 60.0;

            double lengthStart = center.Distance(start);
            double p3hx = start.X + (start.X - center.X) / lengthStart * helperLenght;
            double p3hy = start.Y + (start.Y - center.Y) / lengthStart * helperLenght;

            double lengthEnd = center.Distance(end);
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
                Radius = Size2.Create(offsetX, offsetY),
                IsLargeArc = isLargeArc,
                Angle = angle
            };
        }
    }
}
