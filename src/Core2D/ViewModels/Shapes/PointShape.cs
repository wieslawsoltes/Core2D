using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    public partial class PointShape : BaseShape
    {
        [AutoNotify] private double _x;
        [AutoNotify] private double _y;

        public PointShape() : base(typeof(PointShape))
        {
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
        }

        public override void DrawShape(object dc, IShapeRenderer renderer)
        {
            if (State.Flags.HasFlag(ShapeStateFlags.Visible))
            {
                renderer.DrawPoint(dc, this);
            }
        }

        public override void DrawPoints(object dc, IShapeRenderer renderer)
        {
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
        }

        public override void Move(ISelection selection, decimal dx, decimal dy)
        {
            X = (double)((decimal)X + dx);
            Y = (double)((decimal)Y + dy);
        }

        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(this);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public PointShape Clone()
        {
            var properties = ImmutableArray.Create<Property>();

            // The property Value is of type object and is not cloned.
            if (Properties.Length > 0)
            {
                var builder = properties.ToBuilder();
                foreach (var property in Properties)
                {
                    builder.Add(
                        new Property()
                        {
                            Name = property.Name,
                            Value = property.Value,
                            Owner = this
                        });
                }
                properties = builder.ToImmutable();
            }

            return new PointShape()
            {
                Name = Name,
                Style = Style,
                Properties = properties,
                X = X,
                Y = Y
            };
        }

        public string ToXamlString()
            => $"{_x.ToString(CultureInfo.InvariantCulture)},{_y.ToString(CultureInfo.InvariantCulture)}";

        public string ToSvgString()
            => $"{_x.ToString(CultureInfo.InvariantCulture)},{_y.ToString(CultureInfo.InvariantCulture)}";
    }
}
