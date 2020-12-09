using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class QuadraticBezierBounds : IBounds
    {
        private List<PointShapeViewModel> _points = new List<PointShapeViewModel>();
        public Type TargetType => typeof(QuadraticBezierShapeViewModel);

        public PointShapeViewModel TryToGetPoint(BaseShapeViewModel shapeViewModel, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shapeViewModel is QuadraticBezierShapeViewModel quadratic))
            {
                throw new ArgumentNullException(nameof(shapeViewModel));
            }

            var pointHitTest = registered[typeof(PointShapeViewModel)];

            if (pointHitTest.TryToGetPoint(quadratic.Point1, target, radius, scale, registered) != null)
            {
                return quadratic.Point1;
            }

            if (pointHitTest.TryToGetPoint(quadratic.Point2, target, radius, scale, registered) != null)
            {
                return quadratic.Point2;
            }

            if (pointHitTest.TryToGetPoint(quadratic.Point3, target, radius, scale, registered) != null)
            {
                return quadratic.Point3;
            }

            return null;
        }

        public bool Contains(BaseShapeViewModel shapeViewModel, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shapeViewModel is QuadraticBezierShapeViewModel quadratic))
            {
                throw new ArgumentNullException(nameof(shapeViewModel));
            }

            _points.Clear();
            quadratic.GetPoints(_points);

            if (quadratic.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
            {
                return HitTestHelper.Contains(_points, target, scale);
            }
            else
            {
                return HitTestHelper.Contains(_points, target, 1.0);
            }
        }

        public bool Overlaps(BaseShapeViewModel shapeViewModel, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shapeViewModel is QuadraticBezierShapeViewModel quadratic))
            {
                throw new ArgumentNullException(nameof(shapeViewModel));
            }

            _points.Clear();
            quadratic.GetPoints(_points);

            if (quadratic.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
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
