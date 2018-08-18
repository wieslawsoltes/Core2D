// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Point shape.
    /// </summary>
    public class PointShape : BaseShape, IPointShape
    {
        private double _x;
        private double _y;
        private PointAlignment _alignment;
        private IBaseShape _shape;

        /// <inheritdoc/>
        public override Type TargetType => typeof(IPointShape);

        /// <inheritdoc/>
        public double X
        {
            get => _x;
            set => Update(ref _x, value);
        }

        /// <inheritdoc/>
        public double Y
        {
            get => _y;
            set => Update(ref _y, value);
        }

        /// <inheritdoc/>
        public PointAlignment Alignment
        {
            get => _alignment;
            set => Update(ref _alignment, value);
        }

        /// <inheritdoc/>
        public IBaseShape Shape
        {
            get => _shape;
            set => Update(ref _shape, value);
        }

        /// <inheritdoc/>
        public override void Draw(object dc, ShapeRenderer renderer, double dx, double dy, object db, object r)
        {
            var record = Data?.Record ?? r;

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
        public override void Move(ISet<IBaseShape> selected, double dx, double dy)
        {
            X += dx;
            Y += dy;
        }

        /// <inheritdoc/>
        public override IEnumerable<IPointShape> GetPoints()
        {
            yield return this;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="PointShape"/> instance.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <param name="shape">The point template.</param>
        /// <param name="alignment">The point alignment.</param>
        /// <param name="name">The shape name.</param>
        /// <returns>The new instance of the <see cref="PointShape"/> class.</returns>
        public static IPointShape Create(double x = 0.0, double y = 0.0, IBaseShape shape = null, PointAlignment alignment = PointAlignment.None, string name = "")
        {
            return new PointShape()
            {
                Name = name,
                Style = default,
                X = x,
                Y = y,
                Alignment = alignment,
                Shape = shape
            };
        }

        /// <summary>
        /// Clone current instance of the <see cref="PointShape"/>.
        /// </summary>
        /// <returns>The new instance of the <see cref="PointShape"/> class.</returns>
        public IPointShape Clone()
        {
            var data = Context.Create(Data.Record);

            // The property Value is of type object and is not cloned.
            if (Data.Properties.Length > 0)
            {
                var builder = data.Properties.ToBuilder();
                foreach (var property in Data.Properties)
                {
                    builder.Add(
                        Property.Create(
                            data,
                            property.Name,
                            property.Value));
                }
                data.Properties = builder.ToImmutable();
            }

            return new PointShape()
            {
                Name = Name,
                Style = Style,
                Data = data,
                X = X,
                Y = Y,
                Alignment = Alignment,
                Shape = Shape
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
        public virtual bool ShouldSerializeX() => _x != default;

        /// <summary>
        /// Check whether the <see cref="Y"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeY() => _y != default;

        /// <summary>
        /// Check whether the <see cref="Alignment"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeAlignment() => _alignment != default;

        /// <summary>
        /// Check whether the <see cref="Shape"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeShape() => _shape != null;
    }
}
