using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class EllipseBounds : IBounds
    {
        public Type TargetType => typeof(EllipseShapeViewModel);

        public PointShapeViewModel TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is EllipseShapeViewModel ellipse))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(PointShapeViewModel)];

            if (pointHitTest.TryToGetPoint(ellipse.TopLeft, target, radius, scale, registered) != null)
            {
                return ellipse.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(ellipse.BottomRight, target, radius, scale, registered) != null)
            {
                return ellipse.BottomRight;
            }

            return null;
        }

        public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is EllipseShapeViewModel ellipse))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var rect = Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y);

            if (ellipse.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
            {
                return HitTestHelper.Inflate(ref rect, scale).Contains(target);
            }
            else
            {
                return rect.Contains(target);
            }
        }

        public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is EllipseShapeViewModel ellipse))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var rect = Rect2.FromPoints(
                ellipse.TopLeft.X,
                ellipse.TopLeft.Y,
                ellipse.BottomRight.X,
                ellipse.BottomRight.Y);

            if (ellipse.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
            {
                return HitTestHelper.Inflate(ref rect, scale).IntersectsWith(target);
            }
            else
            {
                return rect.IntersectsWith(target);
            }
        }
    }
}
