﻿#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;
using Core2D.Spatial;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class EllipseDrawNode : DrawNode, IEllipseDrawNode
{
    public EllipseShapeViewModel Ellipse { get; set; }
    public SKRect Rect { get; set; }

    public EllipseDrawNode(EllipseShapeViewModel ellipse, ShapeStyleViewModel? style)
    {
        Style = style;
        Ellipse = ellipse;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = Ellipse.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Ellipse.State.HasFlag(ShapeStateFlags.Size);

        if (Ellipse.TopLeft is { } && Ellipse.BottomRight is { })
        {
            var rect2 = Rect2.FromPoints(Ellipse.TopLeft.X, Ellipse.TopLeft.Y, Ellipse.BottomRight.X, Ellipse.BottomRight.Y);
            Rect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);
            Center = new SKPoint(Rect.MidX, Rect.MidY);
        }
        else
        {
            Rect = SKRect.Empty;
            Center = SKPoint.Empty;
        }
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        if (Ellipse.IsFilled)
        {
            canvas.DrawOval(Rect, Fill);
        }

        if (Ellipse.IsStroked)
        {
            canvas.DrawOval(Rect, Stroke);
        }
    }
}
