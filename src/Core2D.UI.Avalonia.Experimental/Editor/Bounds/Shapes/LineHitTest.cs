// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Shape;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class LineHitTest : HitTestBase
    {
        public override Type TargetType => typeof(LineShape);

        public override PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var line = shape as LineShape;
            if (line == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];

            if (pointHitTest.TryToGetPoint(line.StartPoint, target, radius, hitTest) != null)
            {
                return line.StartPoint;
            }

            if (pointHitTest.TryToGetPoint(line.Point, target, radius, hitTest) != null)
            {
                return line.Point;
            }

            foreach (var point in line.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public override BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var line = shape as LineShape;
            if (line == null)
                throw new ArgumentNullException("shape");

            var a = new Point2(line.StartPoint.X, line.StartPoint.Y);
            var b = new Point2(line.Point.X, line.Point.Y);
            var nearest = target.NearestOnLine(a, b);
            double distance = target.DistanceTo(nearest);
            return distance < radius ? shape : null;
        }

        public override BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var line = shape as LineShape;
            if (line == null)
                throw new ArgumentNullException("shape");

            var a = new Point2(line.StartPoint.X, line.StartPoint.Y);
            var b = new Point2(line.Point.X, line.Point.Y);
            return Line2.LineIntersectsWithRect(
                a, b,
                target,
                out double x0clip, out double y0clip,
                out double x1clip, out double y1clip) ? shape : null;
        }
    }
}
