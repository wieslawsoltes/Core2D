using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public override void Draw(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (_shape != null)
            {
                if (State.Flags.HasFlag(ShapeStateFlags.Visible))
                {
                    var state = base.BeginTransform(dc, renderer);

                    _shape.Draw(dc, renderer, X + dx, Y + dy);

                    base.EndTransform(dc, renderer, state);
                }
            }
        }

        /// <inheritdoc/>
        public override void Bind(IDataFlow dataFlow, object db, object r)
        {
            if (_shape != null)
            {
                var record = Data?.Record ?? r;

                _shape.Bind(dataFlow, db, record);
            }
        }

        /// <inheritdoc/>
        public override void Move(ISelection selection, double dx, double dy)
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
        /// Clone current instance of the <see cref="PointShape"/>.
        /// </summary>
        /// <returns>The new instance of the <see cref="PointShape"/> class.</returns>
        public IPointShape Clone()
        {
            var data = new Context()
            {
                Properties = ImmutableArray.Create<IProperty>(),
                Record = Data.Record
            };

            // The property Value is of type object and is not cloned.
            if (Data.Properties.Length > 0)
            {
                var builder = data.Properties.ToBuilder();
                foreach (var property in Data.Properties)
                {
                    builder.Add(
                        new Property()
                        {
                            Name = property.Name,
                            Value = property.Value,
                            Owner = data
                        });
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

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
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
