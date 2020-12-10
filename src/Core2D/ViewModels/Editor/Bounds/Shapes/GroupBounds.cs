using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class GroupBounds : IBounds
    {
        public Type TargetType => typeof(GroupShapeViewModel);

        public PointShapeViewModel TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is GroupShapeViewModel group))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(PointShapeViewModel)];

            foreach (var groupPoint in group.Connectors.Reverse())
            {
                if (pointHitTest.TryToGetPoint(groupPoint, target, radius, scale, registered) != null)
                {
                    return groupPoint;
                }
            }

            return null;
        }

        public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is GroupShapeViewModel group))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var hasSize = group.State.HasFlag(ShapeStateFlags.Size);

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

        public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is GroupShapeViewModel group))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var hasSize = group.State.HasFlag(ShapeStateFlags.Size);

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
