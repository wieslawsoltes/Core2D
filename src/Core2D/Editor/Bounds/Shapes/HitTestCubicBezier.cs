// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class HitTestCubicBezier : HitTestBase
    {
        public override Type TargetType { get { return typeof(XCubicBezier); } }

        public override XPoint TryToGetPoint(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var cubic = shape as XCubicBezier;
            if (cubic == null)
                throw new ArgumentNullException(nameof(shape));

            var pointHitTest = registered[typeof(XPoint)];

            if (pointHitTest.TryToGetPoint(cubic.Point1, target, radius, registered) != null)
            {
                return cubic.Point1;
            }

            if (pointHitTest.TryToGetPoint(cubic.Point2, target, radius, registered) != null)
            {
                return cubic.Point2;
            }

            if (pointHitTest.TryToGetPoint(cubic.Point3, target, radius, registered) != null)
            {
                return cubic.Point3;
            }

            if (pointHitTest.TryToGetPoint(cubic.Point4, target, radius, registered) != null)
            {
                return cubic.Point4;
            }

            return null;
        }

        public override bool Contains(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var cubic = shape as XCubicBezier;
            if (cubic == null)
                throw new ArgumentNullException(nameof(shape));

            return HitTestHelper.Contains(cubic.GetPoints(), target);
        }

        public override bool Overlaps(BaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var cubic = shape as XCubicBezier;
            if (cubic == null)
                throw new ArgumentNullException(nameof(shape));

            return HitTestHelper.Overlap(cubic.GetPoints(), target);
        }
    }
}
