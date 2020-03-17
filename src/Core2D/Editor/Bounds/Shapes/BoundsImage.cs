// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class BoundsImage : IBounds
    {
        public Type TargetType => typeof(IImageShape);

        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IImageShape image))
                throw new ArgumentNullException(nameof(shape));

            var pointHitTest = registered[typeof(IPointShape)];

            if (pointHitTest.TryToGetPoint(image.TopLeft, target, radius, registered) != null)
            {
                return image.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(image.BottomRight, target, radius, registered) != null)
            {
                return image.BottomRight;
            }

            return null;
        }

        public bool Contains(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IImageShape image))
                throw new ArgumentNullException(nameof(shape));

            return Rect2.FromPoints(
                image.TopLeft.X,
                image.TopLeft.Y,
                image.BottomRight.X,
                image.BottomRight.Y).Contains(target);
        }

        public bool Overlaps(IBaseShape shape, Rect2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IImageShape image))
                throw new ArgumentNullException(nameof(shape));

            return Rect2.FromPoints(
                image.TopLeft.X,
                image.TopLeft.Y,
                image.BottomRight.X,
                image.BottomRight.Y).IntersectsWith(target);
        }
    }
}
