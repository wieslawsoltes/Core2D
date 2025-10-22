// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal class FillDrawNode : DrawNode, IFillDrawNode
{
    public A.Rect Rect { get; set; }
    public BaseColorViewModel? Color { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public FillDrawNode(double x, double y, double width, double height, BaseColorViewModel? color)
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
        Rect = new A.Rect(X, Y, Width, Height);
        Center = Rect.Center;
    }

    public override void UpdateStyle()
    {
        if (Color is { })
        {
            Fill = AvaloniaDrawUtil.ToBrush(Color);
        }
    }

    public override void Draw(object? dc, double zoom)
    {
        OnDraw(dc, zoom);
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not AM.DrawingContext context)
        {
            return;
        }

        if (Rect != default)
        {
            context.DrawRectangle(Fill, null, Rect);
        }
    }
}
