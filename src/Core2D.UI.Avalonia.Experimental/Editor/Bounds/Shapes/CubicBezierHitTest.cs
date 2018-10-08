// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Shape;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class CubicBezierHitTest : HitTestBase
    {
        public override Type TargetType => typeof(CubicBezierShape);

        public override PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var cubicBezier = shape as CubicBezierShape;
            if (cubicBezier == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];

            if (pointHitTest.TryToGetPoint(cubicBezier.StartPoint, target, radius, hitTest) != null)
            {
                return cubicBezier.StartPoint;
            }

            if (pointHitTest.TryToGetPoint(cubicBezier.Point1, target, radius, hitTest) != null)
            {
                return cubicBezier.Point1;
            }

            if (pointHitTest.TryToGetPoint(cubicBezier.Point2, target, radius, hitTest) != null)
            {
                return cubicBezier.Point2;
            }

            if (pointHitTest.TryToGetPoint(cubicBezier.Point3, target, radius, hitTest) != null)
            {
                return cubicBezier.Point3;
            }

            foreach (var point in cubicBezier.Points)
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
            var cubicBezier = shape as CubicBezierShape;
            if (cubicBezier == null)
                throw new ArgumentNullException("shape");

            return HitTestHelper.Contains(cubicBezier.GetPoints(), target) ? shape : null;
        }

        public override BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var cubicBezier = shape as CubicBezierShape;
            if (cubicBezier == null)
                throw new ArgumentNullException("shape");

            return HitTestHelper.Overlap(cubicBezier.GetPoints(), target) ? shape : null;
        }
    }
}
