// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Shapes.Interfaces;
using Core2D.Style;

namespace Core2D.Shapes
{
    /// <summary>
    /// Text shape.
    /// </summary>
    public class TextShape : BaseShape, ITextShape
    {
        private PointShape _topLeft;
        private PointShape _bottomRight;
        private string _text;

        /// <summary>
        /// Gets or sets top-left corner point.
        /// </summary>
        public PointShape TopLeft
        {
            get => _topLeft;
            set => Update(ref _topLeft, value);
        }

        /// <summary>
        /// Gets or sets bottom-right corner point.
        /// </summary>
        public PointShape BottomRight
        {
            get => _bottomRight;
            set => Update(ref _bottomRight, value);
        }

        /// <summary>
        /// Gets or sets text string.
        /// </summary>
        public string Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var state = base.BeginTransform(dc, renderer);

            var record = Data?.Record ?? r;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.Draw(dc, this, dx, dy, db, record);
            }

            if (renderer.State.SelectedShape != null)
            {
                if (this == renderer.State.SelectedShape)
                {
                    _topLeft.Draw(dc, renderer, dx, dy, db, record);
                    _bottomRight.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_topLeft == renderer.State.SelectedShape)
                {
                    _topLeft.Draw(dc, renderer, dx, dy, db, record);
                }
                else if (_bottomRight == renderer.State.SelectedShape)
                {
                    _bottomRight.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            if (renderer.State.SelectedShapes != null)
            {
                if (renderer.State.SelectedShapes.Contains(this))
                {
                    _topLeft.Draw(dc, renderer, dx, dy, db, record);
                    _bottomRight.Draw(dc, renderer, dx, dy, db, record);
                }
            }

            base.EndTransform(dc, renderer, state);
        }

        /// <inheritdoc/>
        public override void Move(ISet<IShape> selected, double dx, double dy)
        {
            if (!TopLeft.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                TopLeft.Move(selected, dx, dy);
            }

            if (!BottomRight.State.Flags.HasFlag(ShapeStateFlags.Connector))
            {
                BottomRight.Move(selected, dx, dy);
            }
        }

        /// <inheritdoc/>
        public override void Select(ISet<IShape> selected)
        {
            base.Select(selected);
            TopLeft.Select(selected);
            BottomRight.Select(selected);
        }

        /// <inheritdoc/>
        public override void Deselect(ISet<IShape> selected)
        {
            base.Deselect(selected);
            TopLeft.Deselect(selected);
            BottomRight.Deselect(selected);
        }

        /// <inheritdoc/>
        public override IEnumerable<PointShape> GetPoints()
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
        /// Creates a new <see cref="TextShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="TextShape"/> class.</returns>
        public static TextShape Create(double x1, double y1, double x2, double y2, ShapeStyle style, BaseShape point, string text, bool isStroked = true, string name = "")
        {
            return new TextShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                TopLeft = PointShape.Create(x1, y1, point),
                BottomRight = PointShape.Create(x2, y2, point),
                Text = text
            };
        }

        /// <summary>
        /// Creates a new <see cref="TextShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="TextShape.TopLeft"/> and <see cref="TextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="TextShape.TopLeft"/> and <see cref="TextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="TextShape"/> class.</returns>
        public static TextShape Create(double x, double y, ShapeStyle style, BaseShape point, string text, bool isStroked = true, string name = "")
        {
            return Create(x, y, x, y, style, point, text, isStroked, name);
        }

        /// <summary>
        /// Creates a new <see cref="TextShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="TextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="TextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="text">The text string.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="TextShape"/> class.</returns>
        public static TextShape Create(PointShape topLeft, PointShape bottomRight, ShapeStyle style, BaseShape point, string text, bool isStroked = true, string name = "")
        {
            return new TextShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = text
            };
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
