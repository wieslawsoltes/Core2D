// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shapes.Interfaces;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class HitTestPoint : HitTestBase
    {
        public override Type TargetType => typeof(IPointShape);

        public override IPointShape TryToGetPoint(IShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            if (!(shape is IPointShape point))
                throw new ArgumentNullException(nameof(shape));

            if (Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y))
            {
                return point;
            }

            return null;
        }

        public override bool Contains(IShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            if (!(shape is IPointShape point))
                throw new ArgumentNullException(nameof(shape));

            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y);
        }

        public override bool Overlaps(IShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            if (!(shape is IPointShape point))
                throw new ArgumentNullException(nameof(shape));

            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).IntersectsWith(target);
        }
    }
}
