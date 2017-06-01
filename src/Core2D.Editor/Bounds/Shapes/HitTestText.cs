// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shape;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class HitTestText : HitTestBase
    {
        public override Type TargetType => typeof(XText);

        public override XPoint TryToGetPoint(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var text = shape as XText;
            if (text == null)
                throw new ArgumentNullException(nameof(shape));

            var pointHitTest = registered[typeof(XPoint)];

            if (pointHitTest.TryToGetPoint(text.TopLeft, target, radius, registered) != null)
            {
                return text.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(text.BottomRight, target, radius, registered) != null)
            {
                return text.BottomRight;
            }

            return null;
        }

        public override bool Contains(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var text = shape as XText;
            if (text == null)
                throw new ArgumentNullException(nameof(shape));

            return Rect2.FromPoints(
                text.TopLeft.X,
                text.TopLeft.Y,
                text.BottomRight.X,
                text.BottomRight.Y).Contains(target);
        }

        public override bool Overlaps(BaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var text = shape as XText;
            if (text == null)
                throw new ArgumentNullException(nameof(shape));

            return Rect2.FromPoints(
                text.TopLeft.X,
                text.TopLeft.Y,
                text.BottomRight.X,
                text.BottomRight.Y).IntersectsWith(target);
        }
    }
}
