using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    public partial class LineShape : BaseShape
    {
        [AutoNotify] private PointShape _start;
        [AutoNotify] private PointShape _end;

        public LineShape() : base(typeof(LineShape))
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
            if (!Start.State.HasFlag(ShapeStateFlags.Connector))
            {
                Start.Move(selection, dx, dy);
            }

            if (!End.State.HasFlag(ShapeStateFlags.Connector))
            {
                End.Move(selection, dx, dy);
            }
        }

        public override void Select(ISelection selection)
        {
            base.Select(selection);
            Start.Select(selection);
            End.Select(selection);
        }

        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);
            Start.Deselect(selection);
            End.Deselect(selection);
        }

        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(Start);
            points.Add(End);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Start.IsDirty();
            isDirty |= End.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            Start.Invalidate();
            End.Invalidate();
        }
    }
}
