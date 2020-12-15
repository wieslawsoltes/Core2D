using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;

namespace Core2D.ViewModels.Shapes
{
    public partial class PointShapeViewModel : BaseShapeViewModel
    {
        [AutoNotify] private double _x;
        [AutoNotify] private double _y;

        public PointShapeViewModel(IServiceProvider serviceProvider) : base(serviceProvider, typeof(PointShapeViewModel))
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

        public override void DrawShape(object dc, IShapeRenderer renderer, ISelection selection)
        {
            if (State.HasFlag(ShapeStateFlags.Visible))
            {
                var isSelected = selection.SelectedShapes?.Count > 0 && selection.SelectedShapes.Contains(this);
                var style = isSelected ? renderer.State.SelectedPointStyle : renderer.State.PointStyle;
                var size = renderer.State.PointSize;
                if (style == null || size <= 0.0)
                {
                    return;
                }
                renderer.DrawPoint(dc, this, style);
            }
        }

        public override void DrawPoints(object dc, IShapeRenderer renderer, ISelection selection)
        {
        }

        public override void Bind(DataFlow dataFlow, object db, object r)
        {
        }

        public override void Move(ISelection selection, decimal dx, decimal dy)
        {
            X = (double)((decimal)_x + dx);
            Y = (double)((decimal)_y + dy);
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            points.Add(this);
        }

        public PointShapeViewModel Clone()
        {
            var properties = ImmutableArray.Create<PropertyViewModel>();

            // The property Value is of type object and is not cloned.
            if (Properties.Length > 0)
            {
                var builder = properties.ToBuilder();
                foreach (var property in Properties)
                {
                    builder.Add(
                        new PropertyViewModel(_serviceProvider)
                        {
                            Name = property.Name,
                            Value = property.Value,
                            Owner = this
                        });
                }
                properties = builder.ToImmutable();
            }

            return new PointShapeViewModel(_serviceProvider)
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
