// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shape;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class HitTestQuadraticBezier : HitTestBase
    {
        public override Type TargetType => typeof(XQuadraticBezier); 

        public override XPoint TryToGetPoint(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var quadratic = shape as XQuadraticBezier;
            if (quadratic == null)
                throw new ArgumentNullException(nameof(shape));

            var pointHitTest = registered[typeof(XPoint)];

            if (pointHitTest.TryToGetPoint(quadratic.Point1, target, radius, registered) != null)
            {
                return quadratic.Point1;
            }

            if (pointHitTest.TryToGetPoint(quadratic.Point2, target, radius, registered) != null)
            {
                return quadratic.Point2;
            }

            if (pointHitTest.TryToGetPoint(quadratic.Point3, target, radius, registered) != null)
            {
                return quadratic.Point3;
            }

            return null;
        }

        public override bool Contains(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var quadratic = shape as XQuadraticBezier;
            if (quadratic == null)
                throw new ArgumentNullException(nameof(shape));

            return HitTestHelper.Contains(quadratic.GetPoints(), target);
        }

        public override bool Overlaps(BaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var quadratic = shape as XQuadraticBezier;
            if (quadratic == null)
                throw new ArgumentNullException(nameof(shape));

            return HitTestHelper.Overlap(quadratic.GetPoints(), target);
        }
    }
}
