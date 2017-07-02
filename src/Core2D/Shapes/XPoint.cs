// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Data;
using Spatial;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Style;
using static System.Math;

namespace Core2D.Shapes
{
    /// <summary>
    /// Point shape.
    /// </summary>
    public class XPoint : BaseShape
    {
        private double _x;
        private double _y;
        private PointAlignment _alignment;
        private BaseShape _shape;

        /// <summary>
        /// Gets or sets X coordinate of point.
        /// </summary>
        public double X
        {
            get => _x;
            set => Update(ref _x, value);
        }

        /// <summary>
        /// Gets or sets Y coordinate of point.
        /// </summary>
        public double Y
        {
            get => _y;
            set => Update(ref _y, value);
        }

        /// <summary>
        /// Gets or sets point alignment.
        /// </summary>
        public PointAlignment Alignment
        {
            get => _alignment;
            set => Update(ref _alignment, value);
        }

        /// <summary>
        /// Gets or sets point template shape.
        /// </summary>
        public BaseShape Shape
        {
            get => _shape;
            set => Update(ref _shape, value);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var record = this.Data.Record ?? r;

            if (_shape != null)
            {
                if (State.Flags.HasFlag(ShapeStateFlags.Visible))
                {
                    var state = base.BeginTransform(dc, renderer);

                    _shape.Draw(dc, renderer, X + dx, Y + dy, db, record);

                    base.EndTransform(dc, renderer, state);
                }
            }
        }

        /// <inheritdoc/>
        public override void Move(ISet<BaseShape> selected, double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        /// <inheritdoc/>
        public override IEnumerable<XPoint> GetPoints()
        {
            yield return this;
        }

        /// <summary>
        /// Calculates distance between points.
        /// </summary>
        /// <param name="point">The other point</param>
        /// <returns>The distance between points.</returns>
        public double DistanceTo(XPoint point)
        {
            double dx = this.X - point.X;
            double dy = this.Y - point.Y;
            return Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Creates a new <see cref="XPoint"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="shape">The point template.</param>
        /// <param name="alignment">The point alignment.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="XPoint"/> class.</returns>
        public static XPoint Create(double x = 0.0, double y = 0.0, BaseShape shape = null, PointAlignment alignment = PointAlignment.None, string name = "")
        {
            return new XPoint()
            {
                Name = name,
                Style = default(ShapeStyle),
                X = x,
                Y = y,
                Alignment = alignment,
                Shape = shape
            };
        }

        /// <summary>
        /// Creates a new <see cref="XPoint"/> instance.
        /// </summary>
        /// <param name="point">The source point.</param>
        /// <returns>The new instance of the <see cref="XPoint"/> class.</returns>
        public static XPoint FromPoint2(Point2 point)
        {
            return new XPoint()
            {
                Name = "",
                Style = default(ShapeStyle),
                X = point.X,
                Y = point.Y,
                Alignment = PointAlignment.None,
                Shape = null
            };
        }

        /// <summary>
        /// Clone current instance of the <see cref="XPoint"/>.
        /// </summary>
        /// <returns>The new instance of the <see cref="XPoint"/> class.</returns>
        public XPoint Clone()
        {
            var data = XContext.Create(this.Data.Record);

            // The property Value is of type object and is not cloned.
            if (this.Data.Properties.Length > 0)
            {
                var builder = data.Properties.ToBuilder();
                foreach (var property in this.Data.Properties)
                {
                    builder.Add(
                        XProperty.Create(
                            data,
                            property.Name,
                            property.Value));
                }
                data.Properties = builder.ToImmutable();
            }

            return new XPoint()
            {
                Name = this.Name,
                Style = this.Style,
                Data = data,
                X = this.X,
                Y = this.Y,
                Alignment = this.Alignment,
                Shape = this.Shape
            };
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0},{1}", _x, _y);
        }

        /// <summary>
        /// Check whether the <see cref="X"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeX() => _x != default(double);

        /// <summary>
        /// Check whether the <see cref="Y"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeY() => _y != default(double);

        /// <summary>
        /// Check whether the <see cref="Alignment"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeAlignment() => _alignment != default(PointAlignment);

        /// <summary>
        /// Check whether the <see cref="Shape"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeShape() => _shape != null;
    }
}
