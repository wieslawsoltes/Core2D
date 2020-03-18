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
        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            var state = base.BeginTransform(dc, renderer);

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.Draw(dc, this, dx, dy);
            }

            if (renderer.State.SelectedShape != null)
            {
                if (this == renderer.State.SelectedShape)
                {
                    _start.Draw(dc, renderer, dx, dy);
                    _end.Draw(dc, renderer, dx, dy);
                }
                else if (_start == renderer.State.SelectedShape)
                {
                    _start.Draw(dc, renderer, dx, dy);
                }
                else if (_end == renderer.State.SelectedShape)
                {
                    _end.Draw(dc, renderer, dx, dy);
                }
            }

            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _start.Draw(dc, renderer, dx, dy);
                    _end.Draw(dc, renderer, dx, dy);
                }
            }

            base.EndTransform(dc, renderer, state);
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
        public override IEnumerable<IPointShape> GetPoints()
        {
            yield return Start;
            yield return End;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
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
