using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Text shape.
    /// </summary>
    public class TextShape : BaseShape, ITextShape
    {
        private IPointShape _topLeft;
        private IPointShape _bottomRight;
        private string _text;

        /// <inheritdoc/>
        public override Type TargetType => typeof(ITextShape);

        /// <inheritdoc/>
        public IPointShape TopLeft
        {
            get => _topLeft;
            set => Update(ref _topLeft, value);
        }

        /// <inheritdoc/>
        public IPointShape BottomRight
        {
            get => _bottomRight;
            set => Update(ref _bottomRight, value);
        }

        /// <inheritdoc/>
        public string Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            var state = base.BeginTransform(dc, renderer);

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.Draw(dc, this, dx, dy);
            }

            if (renderer.State.SelectedShape != null && renderer.State.DrawPoints == true)
            {
                if (this == renderer.State.SelectedShape)
                {
                    _topLeft.Draw(dc, renderer, dx, dy);
                    _bottomRight.Draw(dc, renderer, dx, dy);
                }
                else if (_topLeft == renderer.State.SelectedShape)
                {
                    _topLeft.Draw(dc, renderer, dx, dy);
                }
                else if (_bottomRight == renderer.State.SelectedShape)
                {
                    _bottomRight.Draw(dc, renderer, dx, dy);
                }
            }

            if (renderer.State.SelectedShapes != null && renderer.State.DrawPoints == true)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _topLeft.Draw(dc, renderer, dx, dy);
                    _bottomRight.Draw(dc, renderer, dx, dy);
                }
            }

            base.EndTransform(dc, renderer, state);
        }

        /// <inheritdoc/>
        public override void Bind(IDataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            dataFlow.Bind(this, db, record);

            _topLeft.Bind(dataFlow, db, record);
            _bottomRight.Bind(dataFlow, db, record);
        }

        /// <inheritdoc/>
        public override void Move(ISelection selection, double dx, double dy)
        {
            if (!TopLeft.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                TopLeft.Move(selection, dx, dy);
            }

            if (!BottomRight.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                BottomRight.Move(selection, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISelection selection)
        {
            base.Select(selection);
            TopLeft.Select(selection);
            BottomRight.Select(selection);
        }

        /// <inheritdoc/>
        public override void Deselect(ISelection selection)
        {
            base.Deselect(selection);
            TopLeft.Deselect(selection);
            BottomRight.Deselect(selection);
        }

        /// <inheritdoc/>
        public override IEnumerable<IPointShape> GetPoints()
        {
            yield return TopLeft;
            yield return BottomRight;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether the <see cref="TopLeft"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeTopLeft() => _topLeft != null;

        /// <summary>
        /// Check whether the <see cref="BottomRight"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeBottomRight() => _bottomRight != null;

        /// <summary>
        /// Check whether the <see cref="Text"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeText() => !string.IsNullOrWhiteSpace(_text);
    }
}
