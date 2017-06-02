// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Style;

namespace Core2D.Shapes
{
    /// <summary>
    /// Rectangle shape.
    /// </summary>
    public class XRectangle : XText
    {
        private bool _isGrid;
        private double _offsetX;
        private double _offsetY;
        private double _cellWidth;
        private double _cellHeight;

        /// <summary>
        /// Gets or sets flag indicating whether shape is grid.
        /// </summary>
        public bool IsGrid
        {
            get => _isGrid;
            set => Update(ref _isGrid, value);
        }

        /// <summary>
        /// Gets or sets grid X coordinate offset.
        /// </summary>
        public double OffsetX
        {
            get => _offsetX;
            set => Update(ref _offsetX, value);
        }

        /// <summary>
        /// Gets or sets grid Y coordinate offset.
        /// </summary>
        public double OffsetY
        {
            get => _offsetY;
            set => Update(ref _offsetY, value);
        }

        /// <summary>
        /// Gets or sets grid cell width.
        /// </summary>
        public double CellWidth
        {
            get => _cellWidth;
            set => Update(ref _cellWidth, value);
        }

        /// <summary>
        /// Gets or sets grid cell height.
        /// </summary>
        public double CellHeight
        {
            get => _cellHeight;
            set => Update(ref _cellHeight, value);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, ImmutableArray<XProperty> db, XRecord r)
        {
            var record = this.Data.Record ?? r;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                var state = base.BeginTransform(dc, renderer);

                renderer.Draw(dc, this, dx, dy, db, record);

                base.EndTransform(dc, renderer, state);

                base.Draw(dc, renderer, dx, dy, db, record);
            }
        }

        /// <summary>
        /// Creates a new <see cref="XRectangle"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XRectangle"/> class.</returns>
        public static XRectangle Create(double x1, double y1, double x2, double y2, ShapeStyle style, BaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "")
        {
            return new XRectangle()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                TopLeft = XPoint.Create(x1, y1, point),
                BottomRight = XPoint.Create(x2, y2, point),
                Text = text,
                IsGrid = false,
                OffsetX = 30.0,
                OffsetY = 30.0,
                CellWidth = 30.0,
                CellHeight = 30.0
            };
        }

        /// <summary>
        /// Creates a new <see cref="XRectangle"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="XText.TopLeft"/> and <see cref="XText.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="XText.TopLeft"/> and <see cref="XText.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XRectangle"/> class.</returns>
        public static XRectangle Create(double x, double y, ShapeStyle style, BaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "")
        {
            return Create(x, y, x, y, style, point, isStroked, isFilled, text, name);
        }

        /// <summary>
        /// Creates a new <see cref="XRectangle"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="XText.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="XText.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XRectangle"/> class.</returns>
        public static XRectangle Create(XPoint topLeft, XPoint bottomRight, ShapeStyle style, BaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "")
        {
            return new XRectangle()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = text,
                IsGrid = false,
                OffsetX = 30.0,
                OffsetY = 30.0,
                CellWidth = 30.0,
                CellHeight = 30.0
            };
        }

        /// <summary>
        /// Check whether the <see cref="IsGrid"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsGrid() => _isGrid != default(bool);

        /// <summary>
        /// Check whether the <see cref="OffsetX"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOffsetX() => _offsetX != default(double);

        /// <summary>
        /// Check whether the <see cref="OffsetY"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOffsetY() => _offsetY != default(double);

        /// <summary>
        /// Check whether the <see cref="CellWidth"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCellWidth() => _cellWidth != default(double);

        /// <summary>
        /// Check whether the <see cref="CellHeight"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCellHeight() => _cellHeight != default(double);
    }
}
