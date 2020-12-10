using System;
using System.Collections.Generic;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;

namespace Core2D.ViewModels.Shapes
{
    public partial class RectangleShapeViewModel : BaseShapeViewModel
    {
        [AutoNotify] private PointShapeViewModel _topLeft;
        [AutoNotify] private PointShapeViewModel _bottomRight;

        public RectangleShapeViewModel() : base(typeof(RectangleShapeViewModel))
        {
        }

        public override void DrawShape(object dc, IShapeRenderer renderer)
        {
            if (State.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawRectangle(dc, this);
            }
        }

        public override void DrawPoints(object dc, IShapeRenderer renderer)
        {
            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _topLeft.DrawShape(dc, renderer);
                    _bottomRight.DrawShape(dc, renderer);
                }
                else
                {
                    if (renderer.State.SelectedShapes.Contains(_topLeft))
                    {
                        _topLeft.DrawShape(dc, renderer);
                    }

                    if (renderer.State.SelectedShapes.Contains(_bottomRight))
                    {
                        _bottomRight.DrawShape(dc, renderer);
                    }
                }
            }
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
            var record = Record ?? r;

            dataFlow.Bind(this, db, record);

            _topLeft.Bind(dataFlow, db, record);
            _bottomRight.Bind(dataFlow, db, record);
        }

        public override void Move(ISelection selection, decimal dx, decimal dy)
        {
            if (!_topLeft.State.HasFlag(ShapeStateFlags.Connector))
            {
                _topLeft.Move(selection, dx, dy);
            }

            if (!_bottomRight.State.HasFlag(ShapeStateFlags.Connector))
            {
                _bottomRight.Move(selection, dx, dy);
            }
        }

        public override void Select(ISelection selection)
        {
            base.Select(selection);
            _topLeft.Select(selection);
            _bottomRight.Select(selection);
        }

        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);
            _topLeft.Deselect(selection);
            _bottomRight.Deselect(selection);
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            points.Add(_topLeft);
            points.Add(_bottomRight);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= _topLeft.IsDirty();
            isDirty |= _bottomRight.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            _topLeft.Invalidate();
            _bottomRight.Invalidate();
        }
    }
}
