// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;
using Core2D.Spatial;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class RectangleDrawNode : DrawNode, IRectangleDrawNode
{
    public RectangleShapeViewModel Rectangle { get; set; }
    public SKRect Rect { get; set; }

    public RectangleDrawNode(RectangleShapeViewModel rectangle, ShapeStyleViewModel? style)
    {
        Style = style;
        Rectangle = rectangle;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = Rectangle.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Rectangle.State.HasFlag(ShapeStateFlags.Size);

        if (Rectangle.TopLeft is { } && Rectangle.BottomRight is { })
        {
            var rect2 = Rect2.FromPoints(Rectangle.TopLeft.X, Rectangle.TopLeft.Y, Rectangle.BottomRight.X, Rectangle.BottomRight.Y);
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

        if (Rectangle.IsFilled)
        {
            canvas.DrawRect(Rect, Fill);
        }

        if (Rectangle.IsStroked)
        {
            canvas.DrawRect(Rect, Stroke);
        }
    }
}
