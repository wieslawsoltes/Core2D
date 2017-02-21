// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Spatial.Arc
{
    public struct GdiArc
    {
        public readonly double X;
        public readonly double Y;
        public readonly double Width;
        public readonly double Height;
        public readonly double RadiusX;
        public readonly double RadiusY;
        public readonly double StartAngle;
        public readonly double EndAngle;
        public readonly double SweepAngle;

        public GdiArc(Point2 p1, Point2 p2, Point2 p3, Point2 p4)
        {
            double x1 = p1.X;
            double y1 = p1.Y;
            double x2 = p2.X;
            double y2 = p2.Y;
            double x3 = p3.X;
            double y3 = p3.Y;
            double x4 = p4.X;
            double y4 = p4.Y;
            var rect = Rect2.FromPoints(x1, y1, x2, y2);
            double cx = rect.X + rect.Width / 2.0;
            double cy = rect.Y + rect.Height / 2.0;
            double radiusX = cx - rect.X;
            double radiusY = cy - rect.Y;
            double startAngle = (double)Math.Atan2(y3 - cy, x3 - cx);
            double endAngle = (double)Math.Atan2(y4 - cy, x4 - cx);
            double sweepAngle = (endAngle - startAngle) * 180.0 / Math.PI;
            if (sweepAngle < 0)
                sweepAngle += 360;

            startAngle *= 180.0 / Math.PI;
            endAngle *= 180.0 / Math.PI;

            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
            RadiusX = radiusX;
            RadiusY = radiusY;
            StartAngle = startAngle;
            EndAngle = endAngle;
            SweepAngle = sweepAngle;
        }
    }
}
