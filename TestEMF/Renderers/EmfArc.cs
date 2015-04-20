// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test2d;

namespace TestEMF
{
    internal struct EmfArc
    {
        public double X;
        public double Y;
        public double Width;
        public double Height;
        public double StartAngle;
        public double SweepAngle;

        public static EmfArc FromXArc(XArc arc, double dx, double dy)
        {
            double x1 = arc.Point1.X + dx;
            double y1 = arc.Point1.Y + dy;
            double x2 = arc.Point2.X + dx;
            double y2 = arc.Point2.Y + dy;

            double x0 = (x1 + x2) / 2.0;
            double y0 = (y1 + y2) / 2.0;

            double r = Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
            double x = x0 - r;
            double y = y0 - r;
            double width = 2.0 * r;
            double height = 2.0 * r;

            double startAngle = 180.0 / Math.PI * Math.Atan2(y1 - y0, x1 - x0);
            double endAngle = 180.0 / Math.PI * Math.Atan2(y2 - y0, x2 - x0);
            double sweepAngle = Math.Abs(startAngle) + Math.Abs(endAngle);

            return new EmfArc
            {
                X = x,
                Y = y,
                Width = width,
                Height = height,
                StartAngle = startAngle,
                SweepAngle = sweepAngle
            };
        }
    }
}
