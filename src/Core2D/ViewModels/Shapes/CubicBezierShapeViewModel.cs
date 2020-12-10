using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;

namespace Core2D.ViewModels.Shapes
{
    public partial class CubicBezierShapeViewModel : BaseShapeViewModel
    {
        [AutoNotify] private PointShapeViewModel _point1;
        [AutoNotify] private PointShapeViewModel _point2;
        [AutoNotify] private PointShapeViewModel _point3;
        [AutoNotify] private PointShapeViewModel _point4;

        public CubicBezierShapeViewModel() : base(typeof(CubicBezierShapeViewModel))
        {
        }

        public override void DrawShape(object dc, IShapeRenderer renderer)
        {
            if (State.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawCubicBezier(dc, this);
            }
        }

        public override void DrawPoints(object dc, IShapeRenderer renderer)
        {
            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _point1.DrawShape(dc, renderer);
                    _point2.DrawShape(dc, renderer);
                    _point3.DrawShape(dc, renderer);
                    _point4.DrawShape(dc, renderer);
                }
                else
                {
                    if (renderer.State.SelectedShapes.Contains(_point1))
                    {
                        _point1.DrawShape(dc, renderer);
                    }

                    if (renderer.State.SelectedShapes.Contains(_point2))
                    {
                        _point2.DrawShape(dc, renderer);
                    }

                    if (renderer.State.SelectedShapes.Contains(_point3))
                    {
                        _point3.DrawShape(dc, renderer);
                    }

                    if (renderer.State.SelectedShapes.Contains(_point4))
                    {
                        _point4.DrawShape(dc, renderer);
                    }
                }
            }
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
            var record = Record ?? r;

            dataFlow.Bind(this, db, record);

            _point1.Bind(dataFlow, db, record);
            _point2.Bind(dataFlow, db, record);
            _point3.Bind(dataFlow, db, record);
            _point4.Bind(dataFlow, db, record);
        }

        public override void Move(ISelection selection, decimal dx, decimal dy)
        {
            if (!_point1.State.HasFlag(ShapeStateFlags.Connector))
            {
                _point1.Move(selection, dx, dy);
            }

            if (!_point2.State.HasFlag(ShapeStateFlags.Connector))
            {
                _point2.Move(selection, dx, dy);
            }

            if (!_point3.State.HasFlag(ShapeStateFlags.Connector))
            {
                _point3.Move(selection, dx, dy);
            }

            if (!_point4.State.HasFlag(ShapeStateFlags.Connector))
            {
                _point4.Move(selection, dx, dy);
            }
        }

        public override void Select(ISelection selection)
        {
            base.Select(selection);
            _point1.Select(selection);
            _point2.Select(selection);
            _point3.Select(selection);
            _point4.Select(selection);
        }

        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);
            _point1.Deselect(selection);
            _point2.Deselect(selection);
            _point3.Deselect(selection);
            _point4.Deselect(selection);
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            points.Add(_point1);
            points.Add(_point2);
            points.Add(_point3);
            points.Add(_point4);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= _point1.IsDirty();
            isDirty |= _point2.IsDirty();
            isDirty |= _point3.IsDirty();
            isDirty |= _point4.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            _point1.Invalidate();
            _point2.Invalidate();
            _point3.Invalidate();
            _point4.Invalidate();
        }
    }
}
