// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core2D.Spatial.Arc
{
    public struct WpfArc
    {
        public readonly Point2 P1;
        public readonly Point2 P2;
        public readonly Point2 P3;
        public readonly Point2 P4;
        public readonly Rect2 Rect;
        public readonly Point2 Center;
        public readonly Point2 Start;
        public readonly Point2 End;
        public readonly Size2 Radius;
        public readonly bool IsLargeArc;
        public readonly double Angle;

        public WpfArc(Point2 p1, Point2 p2, Point2 p3, Point2 p4)
        {
            var rect = Rect2.FromPoints(p1, p2);
            var center = new Point2(rect.X + rect.Width / 2.0, rect.Y + rect.Height / 2.0);
            double offsetX = center.X - rect.X;
            double offsetY = center.Y - rect.Y;

            double minLenght = (double)Math.Max(offsetX, offsetY);

            double length1 = center.DistanceTo(p3);
            double p3x = p3.X + (p3.X - center.X) / length1 * minLenght;
            double p3y = p3.Y + (p3.Y - center.Y) / length1 * minLenght;

            double length2 = center.DistanceTo(p4);
            double p4x = p4.X + (p4.X - center.X) / length2 * minLenght;
            double p4y = p4.Y + (p4.Y - center.Y) / length2 * minLenght;

            IList<Point2> p3i;
            IList<Point2> p4i;
            Line2.LineIntersectsWithEllipse(center, new Point2(p3x, p3y), rect, true, out p3i);
            Line2.LineIntersectsWithEllipse(center, new Point2(p4x, p4y), rect, true, out p4i);
            Point2 start;
            Point2 end;

            if (p3i != null && p3i.Count == 1)
                start = p3i.FirstOrDefault();
            else
                start = new Point2(p3x, p3y);

            if (p4i != null && p4i.Count == 1)
                end = p4i.FirstOrDefault();
            else
                end = new Point2(p4x, p4y);

            double angle = Line2.AngleBetween(center, start, center, end);
            bool isLargeArc = angle > 180.0;

            double helperLenght = 60.0;

            double lengthStart = center.DistanceTo(start);
            double p3hx = start.X + (start.X - center.X) / lengthStart * helperLenght;
            double p3hy = start.Y + (start.Y - center.Y) / lengthStart * helperLenght;

            double lengthEnd = center.DistanceTo(end);
            double p4hx = end.X + (end.X - center.X) / lengthEnd * helperLenght;
            double p4hy = end.Y + (end.Y - center.Y) / lengthEnd * helperLenght;

            P1 = p1;
            P2 = p2;
            P3 = new Point2(p3hx, p3hy);
            P4 = new Point2(p4hx, p4hy);
            Rect = rect;
            Center = center;
            Start = start;
            End = end;
            Radius = new Size2(offsetX, offsetY);
            IsLargeArc = isLargeArc;
            Angle = angle;
        }
    }
}
