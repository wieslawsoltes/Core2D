using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.ViewModels.Shapes;
using Spatial;

namespace Core2D.ViewModels.Editor.Bounds
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

        public PointShapeViewModel TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale)
        {
            return Registered[shape.TargetType].TryToGetPoint(shape, target, radius, scale, Registered);
        }

        public PointShapeViewModel TryToGetPoint(IEnumerable<BaseShapeViewModel> shapes, Point2 target, double radius, double scale)
        {
            foreach (var shape in shapes)
            {
                var result = TryToGetPoint(shape, target, radius, scale);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale)
        {
            return Registered[shape.TargetType].Contains(shape, target, radius, scale, Registered);
        }

        public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale)
        {
            return Registered[shape.TargetType].Overlaps(shape, target, radius, scale, Registered);
        }

        public BaseShapeViewModel TryToGetShape(IEnumerable<BaseShapeViewModel> shapes, Point2 target, double radius, double scale)
        {
            foreach (var shape in shapes)
            {
                var result = Registered[shape.TargetType].Contains(shape, target, radius, scale, Registered);
                if (result == true)
                {
                    return shape;
                }
            }
            return null;
        }

        public ISet<BaseShapeViewModel> TryToGetShapes(IEnumerable<BaseShapeViewModel> shapes, Rect2 target, double radius, double scale)
        {
            var selected = new HashSet<BaseShapeViewModel>();
            foreach (var shape in shapes)
            {
                var result = Registered[shape.TargetType].Overlaps(shape, target, radius, scale, Registered);
                if (result == true)
                {
                    selected.Add(shape);
                }
            }
            return selected.Count > 0 ? selected : null;
        }
    }
}
