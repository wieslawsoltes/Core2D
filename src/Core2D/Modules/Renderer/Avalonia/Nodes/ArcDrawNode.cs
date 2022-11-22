﻿#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal class ArcDrawNode : DrawNode, IArcDrawNode
{
    public ArcShapeViewModel Arc { get; set; }
    public AP.IGeometryImpl? Geometry { get; set; }

    public ArcDrawNode(ArcShapeViewModel arc, ShapeStyleViewModel? style)
    {
        Style = style;
        Arc = arc;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = Arc.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Arc.State.HasFlag(ShapeStateFlags.Size);
        Geometry = PathGeometryConverter.ToGeometryImpl(Arc);
        Center = Geometry is { } ? Geometry.Bounds.Center : new A.Point();
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not AP.IDrawingContextImpl context)
        {
            return;
        }

        if (Geometry is { })
        {
            context.DrawGeometry(Arc.IsFilled ? Fill : null, Arc.IsStroked ? Stroke : null, Geometry);
        }
    }
}
