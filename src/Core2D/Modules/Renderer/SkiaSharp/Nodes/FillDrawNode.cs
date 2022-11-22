﻿#nullable enable
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Style;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class FillDrawNode : DrawNode, IFillDrawNode
{
    public SKRect Rect { get; set; }
    public BaseColorViewModel? Color { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public FillDrawNode(double x, double y, double width, double height, BaseColorViewModel color)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        Color = color;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = false;
        ScaleSize = false;
        Rect = SKRect.Create((float)X, (float)Y, (float)Width, (float)Height);
        Center = new SKPoint(Rect.MidX, Rect.MidY);
    }

    public override void UpdateStyle()
    {
        if (Color is { })
        {
            Fill = SkiaSharpDrawUtil.ToSKPaintBrush(Color);
        }
    }

    public override void Draw(object? dc, double zoom)
    {
        OnDraw(dc, zoom);
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        if (!Rect.IsEmpty)
        {
            canvas.DrawRect(Rect, Fill);
        }
    }
}
