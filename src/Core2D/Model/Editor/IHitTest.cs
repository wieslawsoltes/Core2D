using System;
using System.Collections.Generic;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor
{
    public interface IHitTest
    {
        IDictionary<Type, IBounds> Registered { get; set; }

        void Register(IBounds hitTest);

        void Register(IEnumerable<IBounds> hitTests);

        PointShapeViewModel TryToGetPoint(BaseShapeViewModel shapeViewModel, Point2 target, double radius, double scale);

        PointShapeViewModel TryToGetPoint(IEnumerable<BaseShapeViewModel> shapes, Point2 target, double radius, double scale);

        bool Contains(BaseShapeViewModel shapeViewModel, Point2 target, double radius, double scale);

        bool Overlaps(BaseShapeViewModel shapeViewModel, Rect2 target, double radius, double scale);

        BaseShapeViewModel TryToGetShape(IEnumerable<BaseShapeViewModel> shapes, Point2 target, double radius, double scale);

        ISet<BaseShapeViewModel> TryToGetShapes(IEnumerable<BaseShapeViewModel> shapes, Rect2 target, double radius, double scale);
    }
}
