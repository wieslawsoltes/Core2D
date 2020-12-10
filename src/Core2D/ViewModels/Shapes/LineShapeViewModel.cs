using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;

namespace Core2D.ViewModels.Shapes
{
    public partial class LineShapeViewModel : BaseShapeViewModel
    {
        [AutoNotify] private PointShapeViewModel _start;
        [AutoNotify] private PointShapeViewModel _end;

        public LineShapeViewModel() : base(typeof(LineShapeViewModel))
        {
        }

        public override void DrawShape(object dc, IShapeRenderer renderer)
        {
            if (State.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawLine(dc, this);
            }
        }

        public override void DrawPoints(object dc, IShapeRenderer renderer)
        {
            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _start.DrawShape(dc, renderer);
                    _end.DrawShape(dc, renderer);
                }
                else
                {
                    if (renderer.State.SelectedShapes.Contains(_start))
                    {
                        _start.DrawShape(dc, renderer);
                    }

                    if (renderer.State.SelectedShapes.Contains(_end))
                    {
                        _end.DrawShape(dc, renderer);
                    }
                }
            }
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
            var record = Record ?? r;

            dataFlow.Bind(this, db, record);

            _start.Bind(dataFlow, db, record);
            _end.Bind(dataFlow, db, record);
        }

        public override void Move(ISelection selection, decimal dx, decimal dy)
        {
            if (!_start.State.HasFlag(ShapeStateFlags.Connector))
            {
                _start.Move(selection, dx, dy);
            }

            if (!_end.State.HasFlag(ShapeStateFlags.Connector))
            {
                _end.Move(selection, dx, dy);
            }
        }

        public override void Select(ISelection selection)
        {
            base.Select(selection);
            _start.Select(selection);
            _end.Select(selection);
        }

        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);
            _start.Deselect(selection);
            _end.Deselect(selection);
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            points.Add(_start);
            points.Add(_end);
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= _start.IsDirty();
            isDirty |= _end.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            _start.Invalidate();
            _end.Invalidate();
        }
    }
}
