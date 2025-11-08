// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.VelloSharp;

namespace Core2D.Rendering;

internal sealed class VelloSharpRenderHost : IRenderHost
{
    private VelloSharpRenderContext? _context;
    private WriteableBitmap? _bitmap;
    private Vector _dpi = new(96, 96);
    private PixelSize _pixelSize;

    public bool Supports(IShapeRenderer? renderer) => renderer is VelloSharpRenderer;

    public object BeginRender(IShapeRenderer renderer, DrawingContext drawingContext, PixelSize pixelSize, double scaling)
    {
        if (!EnsureResources(pixelSize, scaling))
        {
            throw new InvalidOperationException("Unable to create VelloSharp render resources.");
        }

        _context!.ClearScene();
        return _context;
    }

    public void EndRender(IShapeRenderer renderer, object renderContext, DrawingContext drawingContext, PixelSize pixelSize, Rect destination)
    {
        if (_context is null || _bitmap is null)
        {
            return;
        }

        var context = (VelloSharpRenderContext)renderContext;
        var span = context.Render();
        WritePixels(_bitmap, span);

        var source = new Rect(0, 0, _bitmap.PixelSize.Width, _bitmap.PixelSize.Height);
        drawingContext.DrawImage(_bitmap, source, destination);
    }

    public void Dispose()
    {
        _context?.Dispose();
        _bitmap?.Dispose();
        _context = null;
        _bitmap = null;
    }

    private bool EnsureResources(PixelSize pixelSize, double scaling)
    {
        if (pixelSize.Width <= 0 || pixelSize.Height <= 0)
        {
            return false;
        }

        if (_context is null || _pixelSize != pixelSize)
        {
            _context?.Dispose();
            _context = new VelloSharpRenderContext((uint)pixelSize.Width, (uint)pixelSize.Height);
            _pixelSize = pixelSize;
        }

        var requestedDpi = new Vector(96, 96) * scaling;

        if (_bitmap is null || _bitmap.PixelSize != pixelSize || _dpi != requestedDpi)
        {
            _bitmap?.Dispose();
            _bitmap = new WriteableBitmap(
                pixelSize,
                requestedDpi,
                PixelFormats.Rgba8888,
                AlphaFormat.Premul);
            _dpi = requestedDpi;
        }

        return _context is not null && _bitmap is not null;
    }

    private static unsafe void WritePixels(WriteableBitmap target, ReadOnlySpan<byte> source)
    {
        using var frame = target.Lock();
        var height = target.PixelSize.Height;
        var rowBytes = frame.RowBytes;
        var sourceRowBytes = target.PixelSize.Width * 4;

        var destSpan = new Span<byte>((void*)frame.Address, rowBytes * height);

        for (var y = 0; y < height; y++)
        {
            var sourceRow = source.Slice(y * sourceRowBytes, sourceRowBytes);
            var destRow = destSpan.Slice(y * rowBytes, sourceRowBytes);
            sourceRow.CopyTo(destRow);
        }
    }
}
