using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Spatial;

namespace Core2D.ViewModels.Editor.Bounds.Shapes
{
    public partial class QuadraticBezierBounds : IBounds
    {
        private List<PointShapeViewModel> _points = new List<PointShapeViewModel>();
        public Type TargetType => typeof(QuadraticBezierShapeViewModel);

        public PointShapeViewModel TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is QuadraticBezierShapeViewModel quadratic))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(PointShapeViewModel)];

            if (pointHitTest.TryToGetPoint(quadratic.Point1, target, radius, scale, registered) is { })
            {
                return quadratic.Point1;
            }

            if (pointHitTest.TryToGetPoint(quadratic.Point2, target, radius, scale, registered) is { })
            {
                return quadratic.Point2;
            }

            if (pointHitTest.TryToGetPoint(quadratic.Point3, target, radius, scale, registered) is { })
            {
                return quadratic.Point3;
            }

            return null;
        }

        public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is QuadraticBezierShapeViewModel quadratic))
            {
                throw new ArgumentNullException(nameof(shape));
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

        public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is QuadraticBezierShapeViewModel quadratic))
            {
                throw new ArgumentNullException(nameof(shape));
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
