﻿#nullable enable
using Core2D.Modules.Renderer.Avalonia.Media;
using A = Avalonia;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes.Markers;

internal class RectangleMarker : MarkerBase
{
    public A.Rect Rect { get; set; }

    public override void Draw(object? dc)
    {
        if (dc is not AP.IDrawingContextImpl context)
        {
            return;
        }

        if (ShapeViewModel is null)
        {
            return;
        }
        
        using var rotationDisposable = context.PushPreTransform(Rotation);

        if (ShapeViewModel.IsFilled)
        {
            context.DrawRectangle(Brush, null, Rect);
        }

        if (ShapeViewModel.IsStroked)
        {
            context.DrawRectangle(null, Pen, Rect);
        }
    }
}
