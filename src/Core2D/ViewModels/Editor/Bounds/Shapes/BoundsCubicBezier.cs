using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class BoundsCubicBezier : IBounds
    {
        public Type TargetType => typeof(ICubicBezierShape);

        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is ICubicBezierShape cubic))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(IPointShape)];

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

        public bool Contains(IBaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is ICubicBezierShape cubic))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var points = cubic.GetPoints();

            if (cubic.State.Flags.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
            {
                return HitTestHelper.Contains(points, target, scale);
            }
            else
            {
                return HitTestHelper.Contains(points, target, 1.0);
            }
        }

        public bool Overlaps(IBaseShape shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is ICubicBezierShape cubic))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var points = cubic.GetPoints();

            if (cubic.State.Flags.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
            {
                return HitTestHelper.Overlap(points, target, scale);
            }
            else
            {
                return HitTestHelper.Overlap(points, target, 1.0);
            }
        }
    }
}
