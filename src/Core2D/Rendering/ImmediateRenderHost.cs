// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using Avalonia;
using Avalonia.Media;
using Core2D.Model.Renderer;

namespace Core2D.Rendering;

internal sealed class ImmediateRenderHost : IRenderHost
{
    public bool Supports(IShapeRenderer? renderer) => true;

    public object BeginRender(IShapeRenderer renderer, DrawingContext drawingContext, PixelSize pixelSize, double scaling)
        => drawingContext;

    public void EndRender(IShapeRenderer renderer, object renderContext, DrawingContext drawingContext, PixelSize pixelSize, Rect destination)
    {
        // No additional presentation work required for immediate renderers.
    }

    public void Dispose()
    {
    }
}
