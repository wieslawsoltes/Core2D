#nullable enable
using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor.Bounds.Shapes
{
    public class PathBounds : IBounds
    {
        private readonly List<PointShapeViewModel> _points = new();

        public Type TargetType => typeof(PathShapeViewModel);

        public PointShapeViewModel TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (shape is not PathShapeViewModel path)
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(PointShapeViewModel)];

            _points.Clear();
            path.GetPoints(_points);

            foreach (var pathPoint in _points)
            {
                if (pointHitTest.TryToGetPoint(pathPoint, target, radius, scale, registered) is { })
                {
                    return pathPoint;
                }
            }

            return null;
        }

        public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (shape is not PathShapeViewModel path)
            {
                throw new ArgumentNullException(nameof(shape));
            }

            _points.Clear();
            path.GetPoints(_points);

            if (_points.Count > 0)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (path.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
                {
                    return HitTestHelper.Contains(_points, target, scale);
                }
                else
                {
                    return HitTestHelper.Contains(_points, target, 1.0);
                }
            }

            return false;
        }

        public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (shape is not PathShapeViewModel path)
            {
                throw new ArgumentNullException(nameof(shape));
            }

            _points.Clear();
            path.GetPoints(_points);

            if (_points.Count > 0)
            {
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (path.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
                {
                    return HitTestHelper.Overlap(_points, target, scale);
                }
                else
                {
                    return HitTestHelper.Overlap(_points, target, 1.0);
                }
            }

            return false;
        }
    }
}
