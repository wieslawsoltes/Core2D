#nullable disable
using System;
using System.Collections.Generic;
using Core2D.ViewModels.Shapes;
using Spatial;

namespace Core2D.Model.Editor
{
    public interface IHitTest
    {
        IDictionary<Type, IBounds> Registered { get; set; }

        void Register(IBounds hitTest);

        void Register(IEnumerable<IBounds> hitTests);

        PointShapeViewModel TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale);

        PointShapeViewModel TryToGetPoint(IEnumerable<BaseShapeViewModel> shapes, Point2 target, double radius, double scale);

        bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale);

        bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale);

        BaseShapeViewModel TryToGetShape(IEnumerable<BaseShapeViewModel> shapes, Point2 target, double radius, double scale);

        ISet<BaseShapeViewModel> TryToGetShapes(IEnumerable<BaseShapeViewModel> shapes, Rect2 target, double radius, double scale);
    }
}
