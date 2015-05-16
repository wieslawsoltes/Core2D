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
        /// <param name="arc"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static WpfArc FromXArc(XArc arc, double dx, double dy)
        {
            double x1 = arc.Point1.X + dx;
            double y1 = arc.Point1.Y + dy;
            double x2 = arc.Point2.X + dx;
            double y2 = arc.Point2.Y + dy;

            double dX = x2 - x1;
            double dY = y2 - y1;
            double distance = Math.Sqrt(dX * dX + dY * dY);

            return new WpfArc
            {
                Start = new Point(x1, y1),
                End = new Point(x2, y2),
                Radius = new Size(distance / 2.0, distance / 2.0)
            };
        }
    }
}
