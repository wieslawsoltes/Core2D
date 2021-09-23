#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor.Bounds.Shapes
{
    public class GroupBounds : IBounds
    {
        public Type TargetType => typeof(GroupShapeViewModel);

        public PointShapeViewModel TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (shape is not GroupShapeViewModel group)
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(PointShapeViewModel)];

            foreach (var groupPoint in group.Connectors.Reverse())
            {
                if (pointHitTest.TryToGetPoint(groupPoint, target, radius, scale, registered) is { })
                {
                    return groupPoint;
                }
            }

            return null;
        }

        public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (shape is not GroupShapeViewModel group)
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var hasSize = group.State.HasFlag(ShapeStateFlags.Size);

            foreach (var groupShape in group.Shapes.Reverse())
            {
                var hitTest = registered[groupShape.TargetType];
                var result = hitTest.Contains(groupShape, target, radius, hasSize ? scale : 1.0, registered);
                if (result)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (shape is not GroupShapeViewModel group)
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var hasSize = group.State.HasFlag(ShapeStateFlags.Size);

            foreach (var groupShape in group.Shapes.Reverse())
            {
                var hitTest = registered[groupShape.TargetType];
                var result = hitTest.Overlaps(groupShape, target, radius, hasSize ? scale : 1.0, registered);
                if (result)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
