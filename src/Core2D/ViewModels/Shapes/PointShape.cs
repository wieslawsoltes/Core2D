using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Core2D.Data;
using Core2D.Renderer;

namespace Core2D.Shapes
{
    public class PointShape : BaseShape
    {
        private double _x;
        private double _y;
        private PointAlignment _alignment;

        public override Type TargetType => typeof(PointShape);

        public double X
        {
            get => _x;
            set => RaiseAndSetIfChanged(ref _x, value);
        }

        public double Y
        {
            get => _y;
            set => RaiseAndSetIfChanged(ref _y, value);
        }

        public PointAlignment Alignment
        {
            get => _alignment;
            set => RaiseAndSetIfChanged(ref _alignment, value);
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
            var data = new Context()
            {
                Properties = ImmutableArray.Create<Property>(),
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
                Alignment = Alignment
            };
        }

        public string ToXamlString()
            => $"{_x.ToString(CultureInfo.InvariantCulture)},{_y.ToString(CultureInfo.InvariantCulture)}";

        public string ToSvgString()
            => $"{_x.ToString(CultureInfo.InvariantCulture)},{_y.ToString(CultureInfo.InvariantCulture)}";

        public virtual bool ShouldSerializeX() => _x != default;

        public virtual bool ShouldSerializeY() => _y != default;

        public virtual bool ShouldSerializeAlignment() => _alignment != default;
    }
}
