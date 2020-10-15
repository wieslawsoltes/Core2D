using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Image shape.
    /// </summary>
    public class ImageShape : BaseShape
    {
        private PointShape _topLeft;
        private PointShape _bottomRight;
        private string _key;

        /// <inheritdoc/>
        public override Type TargetType => typeof(ImageShape);

        /// <inheritdoc/>
        public PointShape TopLeft
        {
            get => _topLeft;
            set => Update(ref _topLeft, value);
        }

        /// <inheritdoc/>
        public PointShape BottomRight
        {
            get => _bottomRight;
            set => Update(ref _bottomRight, value);
        }

        /// <inheritdoc/>
        public string Key
        {
            get => _key;
            set => Update(ref _key, value);
        }

        /// <inheritdoc/>
        public override void DrawShape(object dc, IShapeRenderer renderer)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawImage(dc, this);
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override void Bind(DataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            dataFlow.Bind(this, db, record);

            _topLeft.Bind(dataFlow, db, record);
            _bottomRight.Bind(dataFlow, db, record);
        }

        /// <inheritdoc/>
        public override void Move(ISelection selection, decimal dx, decimal dy)
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
        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(TopLeft);
            points.Add(BottomRight);
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

            isDirty |= TopLeft.IsDirty();
            isDirty |= BottomRight.IsDirty();

            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();
            TopLeft.Invalidate();
            BottomRight.Invalidate();
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
        /// Check whether the <see cref="Key"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeKey() => !string.IsNullOrWhiteSpace(_key);
    }
}
