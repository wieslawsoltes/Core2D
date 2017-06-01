// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shape;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class HitTestPath : HitTestBase
    {
        public override Type TargetType => typeof(XPath);

        public override XPoint TryToGetPoint(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var path = shape as XPath;
            if (path == null)
                throw new ArgumentNullException(nameof(shape));

            var pointHitTest = registered[typeof(XPoint)];

            foreach (var pathPoint in path.GetPoints())
            {
                if (pointHitTest.TryToGetPoint(pathPoint, target, radius, registered) != null)
                {
                    return pathPoint;
                }
            }

            return null;
        }

        public override bool Contains(BaseShape shape, Point2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var path = shape as XPath;
            if (path == null)
                throw new ArgumentNullException(nameof(shape));

            return HitTestHelper.Contains(path.GetPoints(), target);
        }

        public override bool Overlaps(BaseShape shape, Rect2 target, double radius, IDictionary<Type, HitTestBase> registered)
        {
            var path = shape as XPath;
            if (path == null)
                throw new ArgumentNullException(nameof(shape));

            return HitTestHelper.Overlap(path.GetPoints(), target);
        }
    }
}
