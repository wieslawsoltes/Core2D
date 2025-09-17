#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Renderer;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Shapes;

public partial class PointShapeViewModel : BaseShapeViewModel
{
    [AutoNotify] private double _x;
    [AutoNotify] private double _y;

    public PointShapeViewModel(IServiceProvider? serviceProvider) : base(serviceProvider, typeof(PointShapeViewModel))
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new PointShapeViewModel(ServiceProvider)
        {
            Name = Name,
            State = State,
            Style = _style?.CopyShared(shared),
            IsStroked = IsStroked,
            IsFilled = IsFilled,
            Properties = _properties.CopyShared(shared).ToImmutable(),
            Record = _record,
            X = X,
            Y = Y,
        };

        return copy;
    }

    public override void DrawShape(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        if (!State.HasFlag(ShapeStateFlags.Visible))
        {
            return;
        }

        if (renderer?.State is not { } state)
        {
            return;
        }

        var style = ResolveStyle(state, selection);
        if (style is null)
        {
            return;
        }

        var size = state.PointSize;
        if (size <= 0.0)
        {
            return;
        }

        renderer.DrawPoint(dc, this, style);
    }

    private ShapeStyleViewModel? ResolveStyle(ShapeRendererStateViewModel state, ISelection? selection)
    {
        var selectedShapes = selection?.SelectedShapes;
        var hoveredShape = selection?.HoveredShape;
        var isConnectionHighlight = state.ActiveConnectionPoints.Contains(this);
        var isSelected = selectedShapes is { Count: > 0 } && selectedShapes.Contains(this);
        var isHovered = ReferenceEquals(hoveredShape, this);

        if (isConnectionHighlight)
        {
            if (state.ConnectorHoverStyle is { })
            {
                return state.ConnectorHoverStyle;
            }

            if (state.ConnectorSelectedStyle is { })
            {
                return state.ConnectorSelectedStyle;
            }
        }

        if (State.HasFlag(ShapeStateFlags.Connector))
        {
            var connectorStyle = ResolveConnectorStyle(state, isSelected, isHovered);
            if (connectorStyle is { })
            {
                return connectorStyle;
            }
        }
        else if (isHovered && state.SelectedPointStyle is { })
        {
            return state.SelectedPointStyle;
        }

        if (!State.HasFlag(ShapeStateFlags.Connector)
            && hoveredShape is PointShapeViewModel hoveredPoint
            && !ReferenceEquals(hoveredPoint, this)
            && hoveredPoint.State.HasFlag(ShapeStateFlags.Connector)
            && IsCloseTo(hoveredPoint))
        {
            if (state.ConnectorHoverStyle is { })
            {
                return state.ConnectorHoverStyle;
            }

            if (state.ConnectorSelectedStyle is { })
            {
                return state.ConnectorSelectedStyle;
            }
        }

        if (isSelected && state.SelectedPointStyle is { })
        {
            return state.SelectedPointStyle;
        }

        return state.PointStyle;
    }

    private ShapeStyleViewModel? ResolveConnectorStyle(ShapeRendererStateViewModel state, bool isSelected, bool isHovered)
    {
        if (isHovered)
        {
            if (state.ConnectorHoverStyle is { })
            {
                return state.ConnectorHoverStyle;
            }

            if (state.ConnectorSelectedStyle is { })
            {
                return state.ConnectorSelectedStyle;
            }
        }

        if (isSelected && state.ConnectorSelectedStyle is { })
        {
            return state.ConnectorSelectedStyle;
        }

        if (State.HasFlag(ShapeStateFlags.Input) && state.ConnectorInputStyle is { })
        {
            return state.ConnectorInputStyle;
        }

        if (State.HasFlag(ShapeStateFlags.Output) && state.ConnectorOutputStyle is { })
        {
            return state.ConnectorOutputStyle;
        }

        if (state.ConnectorNoneStyle is { })
        {
            return state.ConnectorNoneStyle;
        }

        return state.PointStyle;
    }

    private bool IsCloseTo(PointShapeViewModel other)
    {
        const double tolerance = 1e-6;
        return Math.Abs(other.X - X) <= tolerance && Math.Abs(other.Y - Y) <= tolerance;
    }

    public override void DrawPoints(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
    }

    public override void Bind(DataFlow dataFlow, object? db, object? r)
    {
    }

    public override void Move(ISelection? selection, decimal dx, decimal dy)
    {
        X = (double)((decimal)_x + dx);
        Y = (double)((decimal)_y + dy);
    }

    public override void GetPoints(IList<PointShapeViewModel> points)
    {
        points.Add(this);
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();
        return isDirty;
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
                    new PropertyViewModel(ServiceProvider)
                    {
                        Name = property.Name,
                        Value = property.Value,
                        Owner = this
                    });
            }
            properties = builder.ToImmutable();
        }

        return new PointShapeViewModel(ServiceProvider)
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
