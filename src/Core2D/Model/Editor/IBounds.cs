using System;
using System.Collections.Generic;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor
{
    public interface IBounds
    {
        Type TargetType { get; }

        PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered);

        bool Contains(BaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered);

        bool Overlaps(BaseShape shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered);
    }
}
