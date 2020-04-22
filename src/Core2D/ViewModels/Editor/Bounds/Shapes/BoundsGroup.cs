using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class BoundsGroup : IBounds
    {
        public Type TargetType => typeof(IGroupShape);

        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IGroupShape group))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(IPointShape)];

            foreach (var groupPoint in group.Connectors.Reverse())
            {
                if (pointHitTest.TryToGetPoint(groupPoint, target, radius, scale, registered) != null)
                {
                    return groupPoint;
                }
            }

            return null;
        }

        public bool Contains(IBaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IGroupShape group))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var hasSize = group.State.Flags.HasFlag(ShapeStateFlags.Size);

            foreach (var GroupShape in group.Shapes.Reverse())
            {
                var hitTest = registered[GroupShape.TargetType];
                var result = hitTest.Contains(GroupShape, target, radius, hasSize ? scale : 1.0, registered);
                if (result == true)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Overlaps(IBaseShape shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is IGroupShape group))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var hasSize = group.State.Flags.HasFlag(ShapeStateFlags.Size);

            foreach (var GroupShape in group.Shapes.Reverse())
            {
                var hitTest = registered[GroupShape.TargetType];
                var result = hitTest.Overlaps(GroupShape, target, radius, hasSize ? scale : 1.0, registered);
                if (result == true)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
