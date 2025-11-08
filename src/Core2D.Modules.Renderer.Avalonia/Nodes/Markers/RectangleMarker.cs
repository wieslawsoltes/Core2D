// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using A = Avalonia;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes.Markers;

internal class RectangleMarker : MarkerBase
{
    public A.Rect Rect { get; set; }

    public override void Draw(object? dc)
    {
        if (dc is not AM.DrawingContext context)
        {
            return;
        }

        if (ShapeViewModel is null)
        {
            return;
        }
        
        using var rotationDisposable = context.PushTransform(Rotation);

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
