// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class BoundsEllipse : IBounds
    {
        public Type TargetType => typeof(IEllipseShape);

        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IEllipseShape ellipse))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(IPointShape)];

            if (pointHitTest.TryToGetPoint(ellipse.TopLeft, target, radius, registered) != null)
            {
                return ellipse.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(ellipse.BottomRight, target, radius, registered) != null)
            {
                return ellipse.BottomRight;
            }

            return null;
        }

        public bool Contains(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IEllipseShape ellipse))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            return Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y).Contains(target);
        }

        public bool Overlaps(IBaseShape shape, Rect2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IEllipseShape ellipse))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            return Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y).IntersectsWith(target);
        }
    }
}
