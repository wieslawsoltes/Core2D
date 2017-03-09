// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Spatial;

namespace Core2D.Editor.Bounds
{
    public class HitTest
    {
        public IDictionary<Type, HitTestBase> Registered { get; set; }

        public HitTest()
        {
            Registered = new Dictionary<Type, HitTestBase>();
        }

        public void Register(HitTestBase hitTest)
        {
            Registered.Add(hitTest.TargetType, hitTest);
        }

        public void Register(IEnumerable<HitTestBase> hitTests)
        {
            foreach (var hitTest in hitTests)
            {
                Registered.Add(hitTest.TargetType, hitTest);
            }
        }

        public XPoint TryToGetPoint(BaseShape shape, Point2 target, double radius)
        {
            return Registered[shape.GetType()].TryToGetPoint(shape, target, radius, Registered);
        }

        public XPoint TryToGetPoint(IEnumerable<BaseShape> shapes, Point2 target, double radius)
        {
            foreach (var shape in shapes)
            {
                var result = TryToGetPoint(shape, target, radius);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public bool Contains(BaseShape shape, Point2 target, double radius)
        {
            return Registered[shape.GetType()].Contains(shape, target, radius, Registered);
        }

        public bool Overlaps(BaseShape shape, Rect2 target, double radius)
        {
            return Registered[shape.GetType()].Overlaps(shape, target, radius, Registered);
        }

        public BaseShape TryToGetShape(IEnumerable<BaseShape> shapes, Point2 target, double radius)
        {
            foreach (var shape in shapes)
            {
                var result = Registered[shape.GetType()].Contains(shape, target, radius, Registered);
                if (result == true)
                {
                    return shape;
                }
            }
            return null;
        }

        public HashSet<BaseShape> TryToGetShapes(IEnumerable<BaseShape> shapes, Rect2 target, double radius)
        {
            var selected = new HashSet<BaseShape>();
            foreach (var shape in shapes)
            {
                var result = Registered[shape.GetType()].Overlaps(shape, target, radius, Registered);
                if (result == true)
                {
                    selected.Add(shape);
                }
            }
            return selected.Count > 0 ? selected : null;
        }
    }
}
