// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class HitTestArc : HitTestBase
    {
        public override Type TargetType { get { return typeof(XArc); } }

        public override XPoint TryToGetPoint(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var arc = shape as XArc;
            if (arc == null)
                throw new ArgumentNullException(nameof(shape));

            var pointHitTest = registered[typeof(XPoint)];

            if (pointHitTest.TryToGetPoint(arc.Point1, target, radius, registered) != null)
            {
                return arc.Point1;
            }

            if (pointHitTest.TryToGetPoint(arc.Point2, target, radius, registered) != null)
            {
                return arc.Point2;
            }

            if (pointHitTest.TryToGetPoint(arc.Point3, target, radius, registered) != null)
            {
                return arc.Point3;
            }

            if (pointHitTest.TryToGetPoint(arc.Point4, target, radius, registered) != null)
            {
                return arc.Point4;
            }

            return null;
        }

        public override bool Contains(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var arc = shape as XArc;
            if (arc == null)
                throw new ArgumentNullException(nameof(shape));

            return ArcBounds(arc).Contains(target);
        }

        public override bool Overlaps(BaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var arc = shape as XArc;
            if (arc == null)
                throw new ArgumentNullException(nameof(shape));

            return ArcBounds(arc).IntersectsWith(target);
        }

        public static Rect2 ArcBounds(XArc arc)
        {
            double x1 = arc.Point1.X;
            double y1 = arc.Point1.Y;
            double x2 = arc.Point2.X;
            double y2 = arc.Point2.Y;

            double x0 = (x1 + x2) / 2.0;
            double y0 = (y1 + y2) / 2.0;

            double r = Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
            double x = x0 - r;
            double y = y0 - r;
            double width = 2.0 * r;
            double height = 2.0 * r;

            return new Rect2(x, y, width, height);
        }
    }
}
