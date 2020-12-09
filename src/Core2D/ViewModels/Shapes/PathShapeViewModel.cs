using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Path;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    public partial class PathShapeViewModel : BaseShapeViewModel
    {
        private List<PointShapeViewModel> _points;

        [AutoNotify] private PathGeometryViewModel _geometryViewModel;

        public PathShapeViewModel() : base(typeof(PathShapeViewModel))
        {
        }

        private void UpdatePoints()
        {
            if (_points == null)
            {
                _points = new List<PointShapeViewModel>();
                GetPoints(_points);
            }
            else
            {
                _points.Clear();
                GetPoints(_points);
            }
        }

        public override void DrawShape(object dc, IShapeRenderer renderer)
        {
            if (State.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawPath(dc, this);
            }
        }

        public override void DrawPoints(object dc, IShapeRenderer renderer)
        {
            if (renderer.StateViewModel.SelectedShapes != null)
            {
                if (renderer.StateViewModel.SelectedShapes.Contains(this))
                {
                    UpdatePoints();

                    foreach (var point in _points)
                    {
                        point.DrawShape(dc, renderer);
                    }
                }
                else
                {
                    UpdatePoints();

                    foreach (var point in _points)
                    {
                        if (renderer.StateViewModel.SelectedShapes.Contains(point))
                        {
                            point.DrawShape(dc, renderer);
                        }
                    }
                }
            }
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
            var record = RecordViewModel ?? r;

            dataFlow.Bind(this, db, record);

            UpdatePoints();

            foreach (var point in _points)
            {
                point.Bind(dataFlow, db, record);
            }
        }

        public override void Move(ISelection selection, decimal dx, decimal dy)
        {
            UpdatePoints();

            foreach (var point in _points)
            {
                point.Move(selection, dx, dy);
            }
        }

        public override void Select(ISelection selection)
        {
            base.Select(selection);

            UpdatePoints();

            foreach (var point in _points)
            {
                point.Select(selection);
            }
        }

        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);

            UpdatePoints();

            foreach (var point in _points)
            {
                point.Deselect(selection);
            }
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            foreach (var figure in GeometryViewModel.Figures)
            {
                figure.GetPoints(points);
            }
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (GeometryViewModel != null)
            {
                isDirty |= GeometryViewModel.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            GeometryViewModel?.Invalidate();
        }

        public string ToXamlString()
            => GeometryViewModel?.ToXamlString();

        public string ToSvgString()
            => GeometryViewModel?.ToSvgString();
    }
}
