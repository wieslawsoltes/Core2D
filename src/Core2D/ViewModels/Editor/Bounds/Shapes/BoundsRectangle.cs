using System;
using System.Collections.Generic;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class BoundsRectangle : IBounds
    {
        public Type TargetType => typeof(IRectangleShape);

        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IRectangleShape rectangle))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(IPointShape)];

            if (pointHitTest.TryToGetPoint(rectangle.TopLeft, target, radius, registered) != null)
            {
                return rectangle.TopLeft;
            }

            if (pointHitTest.TryToGetPoint(rectangle.BottomRight, target, radius, registered) != null)
            {
                return rectangle.BottomRight;
            }

            return null;
        }

        public bool Contains(IBaseShape shape, Point2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IRectangleShape rectangle))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            return Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y).Contains(target);
        }

        public bool Overlaps(IBaseShape shape, Rect2 target, double radius, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IRectangleShape rectangle))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            return Rect2.FromPoints(
                rectangle.TopLeft.X,
                rectangle.TopLeft.Y,
                rectangle.BottomRight.X,
                rectangle.BottomRight.Y).IntersectsWith(target);
        }
    }
}
