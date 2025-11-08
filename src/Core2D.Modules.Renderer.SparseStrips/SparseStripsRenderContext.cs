// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Vello;

namespace Core2D.Modules.Renderer.SparseStrips;

/// <summary>
/// Wraps a Vello <see cref="RenderContext"/> alongside a <see cref="Pixmap"/> target.
/// </summary>
public sealed class SparseStripsRenderContext : IDisposable
{
    public RenderContext Context { get; }

    public Pixmap Pixmap { get; }

    public ushort Width { get; }

    public ushort Height { get; }

    public SparseStripsRenderContext(ushort width, ushort height, RenderSettings? settings = null)
    {
        Width = width;
        Height = height;
        Context = settings is null
            ? new RenderContext(width, height)
            : new RenderContext(width, height, settings.Value);
        Pixmap = new Pixmap(width, height);
    }

    public void RenderToPixmap()
    {
        Context.RenderToPixmap(Pixmap);
    }

    public ReadOnlySpan<byte> GetPixelBytes()
    {
        return Pixmap.GetBytes();
    }

    public void Dispose()
    {
        Pixmap.Dispose();
        Context.Dispose();
    }
}
