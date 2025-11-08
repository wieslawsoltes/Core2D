// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Avalonia;
using Avalonia.Media;
using Core2D.Model.Renderer;

namespace Core2D.Rendering;

internal interface IRenderHost : IDisposable
{
    bool Supports(IShapeRenderer? renderer);

    object BeginRender(IShapeRenderer renderer, DrawingContext drawingContext, PixelSize pixelSize, double scaling);

    void EndRender(IShapeRenderer renderer, object renderContext, DrawingContext drawingContext, PixelSize pixelSize, Rect destination);
}
