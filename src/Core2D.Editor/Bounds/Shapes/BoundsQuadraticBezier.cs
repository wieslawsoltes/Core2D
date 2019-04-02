// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class BoundsQuadraticBezier : IBounds
    {
        public Type TargetType => typeof(IQuadraticBezierShape);

        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IQuadraticBezierShape quadratic))
                throw new ArgumentNullException(nameof(shape));

            var pointHitTest = registered[typeof(IPointShape)];

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

        public bool Contains(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IQuadraticBezierShape quadratic))
                throw new ArgumentNullException(nameof(shape));

            return HitTestHelper.Contains(quadratic.GetPoints(), target);
        }

        public bool Overlaps(IBaseShape shape, Rect2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IQuadraticBezierShape quadratic))
                throw new ArgumentNullException(nameof(shape));

            return HitTestHelper.Overlap(quadratic.GetPoints(), target);
        }
    }
}
