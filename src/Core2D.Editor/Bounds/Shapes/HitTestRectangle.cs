// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class HitTestRectangle : HitTestBase
    {
        public override Type TargetType => typeof(IRectangleShape);

        public override IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            if (!(shape is IRectangleShape rectangle))
                throw new ArgumentNullException(nameof(shape));

            var pointHitTest = registered[typeof(IPointShape)];

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

        public override bool Contains(IBaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            if (!(shape is IRectangleShape rectangle))
                throw new ArgumentNullException(nameof(shape));

            return Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y).Contains(target);
        }

        public override bool Overlaps(IBaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            if (!(shape is IRectangleShape rectangle))
                throw new ArgumentNullException(nameof(shape));

            return Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y).IntersectsWith(target);
        }
    }
}
