using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class BoundsCubicBezier : IBounds
    {
        private List<PointShape> _points = new List<PointShape>();

        public Type TargetType => typeof(CubicBezierShape);

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is CubicBezierShape cubic))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(PointShape)];

            if (pointHitTest.TryToGetPoint(cubic.Point1, target, radius, scale, registered) != null)
            {
                return cubic.Point1;
            }

            if (pointHitTest.TryToGetPoint(cubic.Point2, target, radius, scale, registered) != null)
            {
                return cubic.Point2;
            }

            if (pointHitTest.TryToGetPoint(cubic.Point3, target, radius, scale, registered) != null)
            {
                return cubic.Point3;
            }

            if (pointHitTest.TryToGetPoint(cubic.Point4, target, radius, scale, registered) != null)
            {
                return cubic.Point4;
            }

            return null;
        }

        public bool Contains(BaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is CubicBezierShape cubic))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            _points.Clear();
            cubic.GetPoints(_points);

            if (cubic.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
            {
                return HitTestHelper.Contains(_points, target, scale);
            }
            else
            {
                return HitTestHelper.Contains(_points, target, 1.0);
            }
        }

        public bool Overlaps(BaseShape shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is CubicBezierShape cubic))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            _points.Clear();
            cubic.GetPoints(_points);

            if (cubic.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
            {
                return HitTestHelper.Overlap(_points, target, scale);
            }
            else
            {
                return HitTestHelper.Overlap(_points, target, 1.0);
            }
        }
    }
}
