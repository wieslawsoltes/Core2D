using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Spatial;

namespace Core2D.ViewModels.Editor.Bounds.Shapes
{
    public partial class PathBounds : IBounds
    {
        private List<PointShapeViewModel> _points = new List<PointShapeViewModel>();

        public Type TargetType => typeof(PathShapeViewModel);

        public PointShapeViewModel TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is PathShapeViewModel path))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(PointShapeViewModel)];

            _points.Clear();
            path.GetPoints(_points);

            foreach (var pathPoint in _points)
            {
                if (pointHitTest.TryToGetPoint(pathPoint, target, radius, scale, registered) != null)
                {
                    return pathPoint;
                }
            }

            return null;
        }

        public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is PathShapeViewModel path))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            _points.Clear();
            path.GetPoints(_points);

            if (_points.Count() > 0)
            {
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
            if (!(shape is PathShapeViewModel path))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            _points.Clear();
            path.GetPoints(_points);

            if (_points.Count() > 0)
            {
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
