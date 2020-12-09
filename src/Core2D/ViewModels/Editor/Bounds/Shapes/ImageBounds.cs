using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class ImageBounds : IBounds
    {
        public Type TargetType => typeof(ImageShapeViewModel);

        public PointShapeViewModel TryToGetPoint(BaseShapeViewModel shapeViewModel, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shapeViewModel is ImageShapeViewModel image))
            {
                throw new ArgumentNullException(nameof(shapeViewModel));
            }

            var pointHitTest = registered[typeof(PointShapeViewModel)];

            if (pointHitTest.TryToGetPoint(image.TopLeft, target, radius, scale, registered) != null)
            {
                return image.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(image.BottomRight, target, radius, scale, registered) != null)
            {
                return image.BottomRight;
            }

            return null;
        }

        public bool Contains(BaseShapeViewModel shapeViewModel, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shapeViewModel is ImageShapeViewModel image))
            {
                throw new ArgumentNullException(nameof(shapeViewModel));
            }

            var rect = Rect2.FromPoints(
                image.TopLeft.X,
                image.TopLeft.Y,
                image.BottomRight.X,
                image.BottomRight.Y);

            if (image.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
            {
                return HitTestHelper.Inflate(ref rect, scale).Contains(target);
            }
            else
            {
                return rect.Contains(target);
            }
        }

        public bool Overlaps(BaseShapeViewModel shapeViewModel, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shapeViewModel is ImageShapeViewModel image))
            {
                throw new ArgumentNullException(nameof(shapeViewModel));
            }

            var rect = Rect2.FromPoints(
                image.TopLeft.X,
                image.TopLeft.Y,
                image.BottomRight.X,
                image.BottomRight.Y);

            if (image.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
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
