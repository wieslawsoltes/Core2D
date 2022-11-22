﻿#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal class PathDrawNode : DrawNode, IPathDrawNode
{
    public PathShapeViewModel Path { get; set; }
    public AP.IGeometryImpl? Geometry { get; set; }

    public PathDrawNode(PathShapeViewModel path, ShapeStyleViewModel? style)
    {
        Style = style;
        Path = path;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = Path.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Path.State.HasFlag(ShapeStateFlags.Size);
        Geometry = PathGeometryConverter.ToGeometryImpl(Path, Path.IsFilled);
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
            context.DrawGeometry(Path.IsFilled ? Fill : null, Path.IsStroked ? Stroke : null, Geometry);
        }
    }
}
