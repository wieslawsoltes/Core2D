// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds
{
    public class HitTest : IHitTest
    {
        public IDictionary<Type, IBounds> Registered { get; set; }

        public HitTest()
        {
            Registered = new Dictionary<Type, IBounds>();
        }

        public void Register(IBounds hitTest)
        {
            Registered.Add(hitTest.TargetType, hitTest);
        }

        public void Register(IEnumerable<IBounds> hitTests)
        {
            foreach (var hitTest in hitTests)
            {
                Registered.Add(hitTest.TargetType, hitTest);
            }
        }

        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius)
        {
            return Registered[shape.TargetType].TryToGetPoint(shape, target, radius, Registered);
        }

        public IPointShape TryToGetPoint(IEnumerable<IBaseShape> shapes, Point2 target, double radius)
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

        public bool Contains(IBaseShape shape, Point2 target, double radius)
        {
            return Registered[shape.TargetType].Contains(shape, target, radius, Registered);
        }

        public bool Overlaps(IBaseShape shape, Rect2 target, double radius)
        {
            return Registered[shape.TargetType].Overlaps(shape, target, radius, Registered);
        }

        public IBaseShape TryToGetShape(IEnumerable<IBaseShape> shapes, Point2 target, double radius)
        {
            foreach (var shape in shapes)
            {
                var result = Registered[shape.TargetType].Contains(shape, target, radius, Registered);
                if (result == true)
                {
                    return shape;
                }
            }
            return null;
        }

        public HashSet<IBaseShape> TryToGetShapes(IEnumerable<IBaseShape> shapes, Rect2 target, double radius)
        {
            var selected = new HashSet<IBaseShape>();
            foreach (var shape in shapes)
            {
                var result = Registered[shape.TargetType].Overlaps(shape, target, radius, Registered);
                if (result == true)
                {
                    selected.Add(shape);
                }
            }
            return selected.Count > 0 ? selected : null;
        }
    }
}
