// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Shape;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class HitTestGroup : HitTestBase
    {
        public override Type TargetType => typeof(GroupShape);

        public override PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var group = shape as GroupShape;
            if (group == null)
                throw new ArgumentNullException(nameof(shape));

            var pointHitTest = registered[typeof(PointShape)];

            foreach (var groupPoint in group.Connectors.Reverse())
            {
                if (pointHitTest.TryToGetPoint(groupPoint, target, radius, registered) != null)
                {
                    return groupPoint;
                }
            }

            return null;
        }

        public override bool Contains(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var group = shape as GroupShape;
            if (group == null)
                throw new ArgumentNullException(nameof(shape));

            foreach (var GroupShape in group.Shapes.Reverse())
            {
                var hitTest = registered[GroupShape.GetType()];
                var result = hitTest.Contains(GroupShape, target, radius, registered);
                if (result == true)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool Overlaps(BaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var group = shape as GroupShape;
            if (group == null)
                throw new ArgumentNullException(nameof(shape));

            foreach (var GroupShape in group.Shapes.Reverse())
            {
                var hitTest = registered[GroupShape.GetType()];
                var result = hitTest.Overlaps(GroupShape, target, radius, registered);
                if (result == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
