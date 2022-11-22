﻿#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class CubicBezierDrawNode : DrawNode, ICubicBezierDrawNode
{
    public CubicBezierShapeViewModel CubicBezier { get; set; }
    public SKPath? Geometry { get; set; }

    public CubicBezierDrawNode(CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel? style)
    {
        Style = style;
        CubicBezier = cubicBezier;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = CubicBezier.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = CubicBezier.State.HasFlag(ShapeStateFlags.Size);
        Geometry = PathGeometryConverter.ToSKPath(CubicBezier);
        if (Geometry is { })
        {
            Center = new SKPoint(Geometry.Bounds.MidX, Geometry.Bounds.MidY);
        }
        else
        {
            Center = SKPoint.Empty;
        }
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        if (CubicBezier.IsFilled)
        {
            canvas.DrawPath(Geometry, Fill);
        }

        if (CubicBezier.IsStroked)
        {
            canvas.DrawPath(Geometry, Stroke);
        }
    }
}
