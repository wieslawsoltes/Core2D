// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class BoundsPoint : IBounds
    {
        public Type TargetType => typeof(IPointShape);

        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IPointShape point))
                throw new ArgumentNullException(nameof(shape));

            if (Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y))
            {
                return point;
            }

            return null;
        }

        public bool Contains(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IPointShape point))
                throw new ArgumentNullException(nameof(shape));

            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).Contains(target.X, target.Y);
        }

        public bool Overlaps(IBaseShape shape, Rect2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IPointShape point))
                throw new ArgumentNullException(nameof(shape));

            return Point2.FromXY(point.X, point.Y).ExpandToRect(radius).IntersectsWith(target);
        }
    }
}
