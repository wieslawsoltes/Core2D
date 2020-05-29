using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Line shape.
    /// </summary>
    public class LineShape : BaseShape, ILineShape
    {
        private IPointShape _start;
        private IPointShape _end;

        /// <inheritdoc/>
        public override Type TargetType => typeof(ILineShape);

        /// <inheritdoc/>
        public IPointShape Start
        {
            get => _start;
            set => Update(ref _start, value);
        }

        /// <inheritdoc/>
        public IPointShape End
        {
            get => _end;
            set => Update(ref _end, value);
        }

        /// <inheritdoc/>
        public override void DrawShape(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawLine(dc, this, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void DrawPoints(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (renderer.State.SelectedShapes != null && renderer.State.DrawPoints == true)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _start.DrawShape(dc, renderer, dx, dy);
                    _end.DrawShape(dc, renderer, dx, dy);
                }
                else
                {
                    if (renderer.State.SelectedShapes.Contains(_start))
                    {
                        _start.DrawShape(dc, renderer, dx, dy);
                    }

                    if (renderer.State.SelectedShapes.Contains(_end))
                    {
                        _end.DrawShape(dc, renderer, dx, dy);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public override void Bind(IDataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            dataFlow.Bind(this, db, record);

            _start.Bind(dataFlow, db, record);
            _end.Bind(dataFlow, db, record);
        }

        /// <inheritdoc/>
        public override void Move(ISelection selection, double dx, double dy)
        {
            if (!Start.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                Start.Move(selection, dx, dy);
            }

            if (!End.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                End.Move(selection, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISelection selection)
        {
            base.Select(selection);
            Start.Select(selection);
            End.Select(selection);
        }

        /// <inheritdoc/>
        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);
            Start.Deselect(selection);
            End.Deselect(selection);
        }

        /// <inheritdoc/>
        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(Start);
            points.Add(End);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Start.IsDirty();
            isDirty |= End.IsDirty();

            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();
            Start.Invalidate();
            End.Invalidate();
        }

        /// <summary>
        /// Check whether the <see cref="Start"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeStart() => _start != null;

        /// <summary>
        /// Check whether the <see cref="End"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeEnd() => _end != null;
    }
}
