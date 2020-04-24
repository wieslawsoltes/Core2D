using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Rectangle shape.
    /// </summary>
    public class RectangleShape : TextShape, IRectangleShape
    {
        private bool _isGrid;
        private double _offsetX;
        private double _offsetY;
        private double _cellWidth;
        private double _cellHeight;

        /// <inheritdoc/>
        public override Type TargetType => typeof(IRectangleShape);

        /// <inheritdoc/>
        public bool IsGrid
        {
            get => _isGrid;
            set => Update(ref _isGrid, value);
        }

        /// <inheritdoc/>
        public double OffsetX
        {
            get => _offsetX;
            set => Update(ref _offsetX, value);
        }

        /// <inheritdoc/>
        public double OffsetY
        {
            get => _offsetY;
            set => Update(ref _offsetY, value);
        }

        /// <inheritdoc/>
        public double CellWidth
        {
            get => _cellWidth;
            set => Update(ref _cellWidth, value);
        }

        /// <inheritdoc/>
        public double CellHeight
        {
            get => _cellHeight;
            set => Update(ref _cellHeight, value);
        }

        /// <inheritdoc/>
        public override void DrawShape(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.Draw(dc, this, dx, dy);
                base.DrawShape(dc, renderer, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void DrawPoints(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                base.DrawPoints(dc, renderer, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Bind(IDataFlow dataFlow, object db, object r)
        {
            var record = Data?.Record ?? r;

            dataFlow.Bind(this, db, record);

            base.Bind(dataFlow, db, record);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check whether the <see cref="IsGrid"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsGrid() => _isGrid != default;

        /// <summary>
        /// Check whether the <see cref="OffsetX"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOffsetX() => _offsetX != default;

        /// <summary>
        /// Check whether the <see cref="OffsetY"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOffsetY() => _offsetY != default;

        /// <summary>
        /// Check whether the <see cref="CellWidth"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCellWidth() => _cellWidth != default;

        /// <summary>
        /// Check whether the <see cref="CellHeight"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCellHeight() => _cellHeight != default;
    }
}
