// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Shape;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds
{
    public class HitTest : IHitTest
    {
        public Dictionary<Type, HitTestBase> Registered { get; set; }

        public HitTest()
        {
            Registered = new Dictionary<Type, HitTestBase>();
        }

        public void Register(HitTestBase hitTest)
        {
            Registered.Add(hitTest.TargetType, hitTest);
        }

        private HitTestBase GetHitTest(object target)
        {
            return Registered.TryGetValue(target?.GetType(), out var hitTest) ? hitTest : null;
        }

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius)
        {
            return GetHitTest(shape)?.TryToGetPoint(shape, target, radius, this);
        }

        public PointShape TryToGetPoint(IEnumerable<BaseShape> shapes, Point2 target, double radius, PointShape exclude)
        {
            foreach (var shape in shapes.Reverse())
            {
                var result = TryToGetPoint(shape, target, radius);
                if (result != null && result != exclude)
                {
                    return result;
                }
            }
            return null;
        }

        public BaseShape TryToGetShape(IEnumerable<BaseShape> shapes, Point2 target, double radius)
        {
            foreach (var shape in shapes.Reverse())
            {
                var result = GetHitTest(shape)?.Contains(shape, target, radius, this);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public ISet<BaseShape> TryToGetShapes(IEnumerable<BaseShape> shapes, Rect2 target, double radius)
        {
            var selected = new HashSet<BaseShape>();
            foreach (var shape in shapes.Reverse())
            {
                var result = GetHitTest(shape)?.Overlaps(shape, target, radius, this);
                if (result != null)
                {
                    selected.Add(shape);
                }
            }
            return selected.Count > 0 ? selected : null;
        }
    }
}
