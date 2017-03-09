// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class HitTestImage : HitTestBase
    {
        public override Type TargetType { get { return typeof(XImage); } }

        public override XPoint TryToGetPoint(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var image = shape as XImage;
            if (image == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = registered[typeof(XPoint)];

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

        public override bool Contains(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var image = shape as XImage;
            if (image == null)
                throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                image.TopLeft.X,
                image.TopLeft.Y,
                image.BottomRight.X,
                image.BottomRight.Y).Contains(target);
        }

        public override bool Overlaps(BaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var image = shape as XImage;
            if (image == null)
                throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                image.TopLeft.X,
                image.TopLeft.Y,
                image.BottomRight.X,
                image.BottomRight.Y).IntersectsWith(target);
        }
    }
}
