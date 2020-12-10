using System;
using System.Collections.Generic;
using Core2D.ViewModels.Shapes;
using Spatial;

namespace Core2D.Model.Editor
{
    public interface IBounds
    {
        Type TargetType { get; }

        PointShapeViewModel TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered);

        bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered);

        bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered);
    }
}
