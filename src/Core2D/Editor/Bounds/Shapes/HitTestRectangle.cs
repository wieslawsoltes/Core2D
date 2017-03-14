// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class HitTestRectangle : HitTestBase
    {
        public override Type TargetType => typeof(XRectangle);

        public override XPoint TryToGetPoint(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var rectangle = shape as XRectangle;
            if (rectangle == null)
                throw new ArgumentNullException(nameof(shape));

            var pointHitTest = registered[typeof(XPoint)];

            if (pointHitTest.TryToGetPoint(rectangle.TopLeft, target, radius, registered) != null)
            {
                return rectangle.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(rectangle.BottomRight, target, radius, registered) != null)
            {
                return rectangle.BottomRight;
            }

            return null;
        }

        public override bool Contains(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var rectangle = shape as XRectangle;
            if (rectangle == null)
                throw new ArgumentNullException(nameof(shape));

            return Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y).Contains(target);
        }

        public override bool Overlaps(BaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var rectangle = shape as XRectangle;
            if (rectangle == null)
                throw new ArgumentNullException(nameof(shape));

            return Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y).IntersectsWith(target);
        }
    }
}
