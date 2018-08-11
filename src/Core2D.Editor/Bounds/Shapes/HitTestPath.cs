// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Shapes.Interfaces;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class HitTestPath : HitTestBase
    {
        public override Type TargetType => typeof(IPathShape);

        public override IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            if (!(shape is IPathShape path))
                throw new ArgumentNullException(nameof(shape));

            var pointHitTest = registered[typeof(IPointShape)];

            foreach (var pathPoint in path.GetPoints())
            {
                if (pointHitTest.TryToGetPoint(pathPoint, target, radius, registered) != null)
                {
                    return pathPoint;
                }
            }

            return null;
        }

        public override bool Contains(IBaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            if (!(shape is IPathShape path))
                throw new ArgumentNullException(nameof(shape));

            var points = path.GetPoints();
            if (points.Count() > 0)
                return HitTestHelper.Contains(points, target);
            return false;
        }

        public override bool Overlaps(IBaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            if (!(shape is IPathShape path))
                throw new ArgumentNullException(nameof(shape));

            var points = path.GetPoints();
            if (points.Count() > 0)
                return HitTestHelper.Overlap(points, target);
            return false;
        }
    }
}
