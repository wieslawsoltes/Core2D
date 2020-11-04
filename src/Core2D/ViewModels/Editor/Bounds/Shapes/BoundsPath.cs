using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class BoundsPath : IBounds
    {
        private List<PointShape> _points = new List<PointShape>();

        public Type TargetType => typeof(PathShape);

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is PathShape path))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(PointShape)];

            _points.Clear();
            path.GetPoints(_points);

            foreach (var pathPoint in _points)
            {
                if (pointHitTest.TryToGetPoint(pathPoint, target, radius, scale, registered) != null)
                {
                    return pathPoint;
                }
            }

            return null;
        }

        public bool Contains(BaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is PathShape path))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            _points.Clear();
            path.GetPoints(_points);

            if (_points.Count() > 0)
            {
                if (path.State.Flags.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
                {
                    return HitTestHelper.Contains(_points, target, scale);
                }
                else
                {
                    return HitTestHelper.Contains(_points, target, 1.0);
                }
            }

            return false;
        }

        public bool Overlaps(BaseShape shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is PathShape path))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            _points.Clear();
            path.GetPoints(_points);

            if (_points.Count() > 0)
            {
                if (path.State.Flags.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
                {
                    return HitTestHelper.Overlap(_points, target, scale);
                }
                else
                {
                    return HitTestHelper.Overlap(_points, target, 1.0);
                }
            }

            return false;
        }
    }
}