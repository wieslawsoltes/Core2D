using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class BoundsPath : IBounds
    {
        public Type TargetType => typeof(IPathShape);

        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IPathShape path))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(IPointShape)];

            foreach (var pathPoint in path.GetPoints())
            {
                if (pointHitTest.TryToGetPoint(pathPoint, target, radius, registered) != null)
                {
                    return pathPoint;
                }
            }

            return null;
        }

        public bool Contains(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IPathShape path))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var points = path.GetPoints();
            if (points.Count() > 0)
            {
                return HitTestHelper.Contains(points, target);
            }

            return false;
        }

        public bool Overlaps(IBaseShape shape, Rect2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IPathShape path))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var points = path.GetPoints();
            if (points.Count() > 0)
            {
                return HitTestHelper.Overlap(points, target);
            }

            return false;
        }
    }
}
