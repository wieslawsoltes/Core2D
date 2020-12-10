using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class CubicBezierBounds : IBounds
    {
        private List<PointShapeViewModel> _points = new List<PointShapeViewModel>();

        public Type TargetType => typeof(CubicBezierShapeViewModel);

        public PointShapeViewModel TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is CubicBezierShapeViewModel cubic))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(PointShapeViewModel)];

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

        public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is CubicBezierShapeViewModel cubic))
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

        public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is CubicBezierShapeViewModel cubic))
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
