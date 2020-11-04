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

        PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, double scale);

        PointShape TryToGetPoint(IEnumerable<BaseShape> shapes, Point2 target, double radius, double scale);

        bool Contains(BaseShape shape, Point2 target, double radius, double scale);

        bool Overlaps(BaseShape shape, Rect2 target, double radius, double scale);

        BaseShape TryToGetShape(IEnumerable<BaseShape> shapes, Point2 target, double radius, double scale);

        ISet<BaseShape> TryToGetShapes(IEnumerable<BaseShape> shapes, Rect2 target, double radius, double scale);
    }
}
