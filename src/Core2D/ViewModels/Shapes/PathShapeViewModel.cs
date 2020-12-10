using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Path;

namespace Core2D.ViewModels.Shapes
{
    public partial class PathShapeViewModel : BaseShapeViewModel
    {
        private List<PointShapeViewModel> _points;

        [AutoNotify] private PathGeometryViewModel _geometry;

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
            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
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
                        if (renderer.State.SelectedShapes.Contains(point))
                        {
                            point.DrawShape(dc, renderer);
                        }
                    }
                }
            }
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
            var record = Record ?? r;

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
            foreach (var figure in _geometry.Figures)
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

            if (_geometry != null)
            {
                isDirty |= _geometry.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _geometry?.Invalidate();
        }

        public string ToXamlString()
            => _geometry?.ToXamlString();

        public string ToSvgString()
            => _geometry?.ToSvgString();
    }
}
