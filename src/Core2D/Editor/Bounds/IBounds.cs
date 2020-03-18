using System;
using System.Collections.Generic;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds
{
    public interface IBounds
    {
        Type TargetType { get; }
        IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered);
        bool Contains(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered);
        bool Overlaps(IBaseShape shape, Rect2 target, double radius, IDictionary<Type, IBounds> registered);
    }
}
