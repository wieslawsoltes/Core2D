// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Shape;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class GroupHitTest : HitTestBase
    {
        public override Type TargetType => typeof(GroupShape);

        public override PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var group = shape as GroupShape;
            if (group == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];

            foreach (var groupPoint in group.Points)
            {
                if (pointHitTest.TryToGetPoint(groupPoint, target, radius, hitTest) != null)
                {
                    return groupPoint;
                }
            }

            foreach (var groupShape in group.Shapes)
            {
                var groupHitTest = hitTest.Registered[groupShape.GetType()];
                var result = groupHitTest.TryToGetPoint(groupShape, target, radius, hitTest);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        public override BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var group = shape as GroupShape;
            if (group == null)
                throw new ArgumentNullException("shape");

            foreach (var groupShape in group.Shapes)
            {
                var groupHitTest = hitTest.Registered[groupShape.GetType()];
                var result = groupHitTest.Contains(groupShape, target, radius, hitTest);
                if (result != null)
                {
                    return group;
                }
            }
            return null;
        }

        public override BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var group = shape as GroupShape;
            if (group == null)
                throw new ArgumentNullException("shape");

            foreach (var groupShape in group.Shapes)
            {
                var groupHitTest = hitTest.Registered[groupShape.GetType()];
                var result = groupHitTest.Overlaps(groupShape, target, radius, hitTest);
                if (result != null)
                {
                    return group;
                }
            }
            return null;
        }
    }
}
