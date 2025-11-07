// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using VelloSharp;
using VelloSharp.Rendering;
using VScene = VelloSharp.Scene;
using VSRenderer = VelloSharp.Renderer;

namespace Core2D.Modules.Renderer.VelloSharp;

/// <summary>
/// Wraps a VelloSharp renderer, scene and CPU render target buffer.
/// </summary>
public sealed class VelloSharpRenderContext : IDisposable
{
    public VScene Scene { get; }

    public VSRenderer Renderer { get; }

    public RenderParams RenderParams { get; private set; }

    public RenderTargetDescriptor Target { get; }

    private readonly byte[] _buffer;

    public VelloSharpRenderContext(uint width, uint height)
    {
        if (width == 0 || height == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width));
        }

        Renderer = new VSRenderer(width, height);
        Scene = new VScene();
        Target = new RenderTargetDescriptor(width, height, RenderFormat.Rgba8, checked((int)width * 4));
        RenderParams = new RenderParams(width, height, RgbaColor.FromBytes(0, 0, 0, 0));
        _buffer = GC.AllocateUninitializedArray<byte>(Target.RequiredBufferSize);
    }

    public void Resize(uint width, uint height)
    {
        Renderer.Resize(width, height);
        RenderParams = RenderParams with
        {
            Width = width,
            Height = height
        };
    }

    public ReadOnlySpan<byte> Render()
    {
        VelloRenderPath.Render(Renderer, Scene, _buffer, RenderParams, Target);
        return _buffer;
    }

    public void ClearScene() => Scene.Reset();

    public void Dispose()
    {
        Scene.Dispose();
        Renderer.Dispose();
    }
}
