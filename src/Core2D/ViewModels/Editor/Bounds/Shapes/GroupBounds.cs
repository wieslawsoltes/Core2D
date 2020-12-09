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

        public PointShapeViewModel TryToGetPoint(BaseShapeViewModel shapeViewModel, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shapeViewModel is GroupShapeViewModel group))
            {
                throw new ArgumentNullException(nameof(shapeViewModel));
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

        public bool Contains(BaseShapeViewModel shapeViewModel, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shapeViewModel is GroupShapeViewModel group))
            {
                throw new ArgumentNullException(nameof(shapeViewModel));
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

        public bool Overlaps(BaseShapeViewModel shapeViewModel, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shapeViewModel is GroupShapeViewModel group))
            {
                throw new ArgumentNullException(nameof(shapeViewModel));
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
