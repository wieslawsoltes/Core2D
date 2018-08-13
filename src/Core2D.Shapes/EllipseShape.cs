// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Style;

namespace Core2D.Shapes
{
    /// <summary>
    /// Ellipse shape.
    /// </summary>
    public class EllipseShape : TextShape, IEllipseShape
    {
        /// <inheritdoc/>
        public override Type TargetType => typeof(IEllipseShape);

        /// <inheritdoc/>
        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var record = Data?.Record ?? r;

            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                var state = base.BeginTransform(dc, renderer);

                renderer.Draw(dc, this, dx, dy, db, record);

                base.EndTransform(dc, renderer, state);

                base.Draw(dc, renderer, dx, dy, db, record);
            }
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="EllipseShape"/> instance.
        /// </summary>
        /// <param name="x1">The X coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="y1">The Y coordinate of <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="x2">The X coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="y2">The Y coordinate of <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="EllipseShape"/> class.</returns>
        public static IEllipseShape Create(double x1, double y1, double x2, double y2, ShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "")
        {
            return new EllipseShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                TopLeft = PointShape.Create(x1, y1, point),
                BottomRight = PointShape.Create(x2, y2, point),
                Text = text,
            };
        }

        /// <summary>
        /// Creates a new <see cref="EllipseShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="y">The Y coordinate of <see cref="ITextShape.TopLeft"/> and <see cref="ITextShape.BottomRight"/> corner points.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="EllipseShape"/> class.</returns>
        public static IEllipseShape Create(double x, double y, ShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "")
        {
            return Create(x, y, x, y, style, point, isStroked, isFilled, text, name);
        }

        /// <summary>
        /// Creates a new <see cref="EllipseShape"/> instance.
        /// </summary>
        /// <param name="topLeft">The <see cref="ITextShape.TopLeft"/> corner point.</param>
        /// <param name="bottomRight">The <see cref="ITextShape.BottomRight"/> corner point.</param>
        /// <param name="style">The shape style.</param>
        /// <param name="point">The point template.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isFilled">The flag indicating whether shape is filled.</param>
        /// <param name="text">The text string.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="EllipseShape"/> class.</returns>
        public static IEllipseShape Create(IPointShape topLeft, IPointShape bottomRight, ShapeStyle style, IBaseShape point, bool isStroked = true, bool isFilled = false, string text = null, string name = "")
        {
            return new EllipseShape()
            {
                Name = name,
                Style = style,
                IsStroked = isStroked,
                IsFilled = isFilled,
                TopLeft = topLeft,
                BottomRight = bottomRight,
                Text = text,
            };
        }
    }
}
