// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using ACP = Avalonia.Controls.PanAndZoom;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal abstract class DrawNode : IDrawNode
{
    public ShapeStyleViewModel? Style { get; set; }
    public bool ScaleThickness { get; set; }
    public bool ScaleSize { get; set; }
    public AM.IBrush? Fill { get; set; }
    public AM.IPen? Stroke { get; set; }
    public A.Point Center { get; set; }

    public abstract void UpdateGeometry();

    public virtual void UpdateStyle()
    {
        if (Style?.Fill?.Color is { })
        {
            Fill = AvaloniaDrawUtil.ToBrush(Style.Fill.Color);
        }
        else
        {
            Fill = null;
        }

        if (Style?.Stroke is { })
        {
            Stroke = AvaloniaDrawUtil.ToPen(Style, Style.Stroke.Thickness);
        }
        else
        {
            Stroke = null;
        }
    }

    public virtual void Draw(object? dc, double zoom)
    {
        if (dc is not AM.DrawingContext context)
        {
            return;
        }

        var scale = ScaleSize ? 1.0 / zoom : 1.0;
        var translateX = 0.0 - (Center.X * scale) + Center.X;
        var translateY = 0.0 - (Center.Y * scale) + Center.Y;
        var thickness = Style?.Stroke?.Thickness ?? 1d;

        if (ScaleThickness)
        {
            thickness /= zoom;
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (scale != 1.0)
        {
            thickness /= scale;
        }

        if (Style?.Stroke is { } && Stroke is { })
        {
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Stroke.Thickness != thickness)
            {
                Stroke = AvaloniaDrawUtil.ToPen(Style, thickness);
            }
        }

        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (scale != 1.0)
        {
            using var translateDisposable = context.PushTransform(ACP.MatrixHelper.Translate(translateX, translateY));
            using var scaleDisposable = context.PushTransform(ACP.MatrixHelper.Scale(scale, scale));
            OnDraw(dc, zoom);
        }
        else
        {
            OnDraw(dc, zoom);
        }
    }

    public abstract void OnDraw(object? dc, double zoom);

    public virtual void Dispose()
    {
    }
}
