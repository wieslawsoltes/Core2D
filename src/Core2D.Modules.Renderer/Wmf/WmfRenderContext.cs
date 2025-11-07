// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using Oxage.Wmf;
using Oxage.Wmf.Records;

namespace Core2D.Modules.Renderer.Wmf;

internal sealed class WmfRenderContext : IDisposable
{
    private readonly Dictionary<uint, int> _brushHandles = new();
    private readonly Dictionary<PenKey, int> _penHandles = new();
    private readonly List<int> _handleOrder = new();
    private int _nextHandle;
    private int _nullBrushHandle = -1;
    private int _nullPenHandle = -1;
    private bool _disposed;

    public WmfRenderContext(int width, int height)
    {
        Document = new WmfDocument
        {
            Width = width,
            Height = height
        };

        Document.AddPolyFillMode(PolyFillMode.WINDING);
    }

    public WmfDocument Document { get; }

    public void SelectBrush(Color color, bool isVisible)
    {
        var handle = isVisible ? EnsureBrush(color) : EnsureNullBrush();
        Document.AddSelectObject(handle);
    }

    public void SelectPen(Color color, double thickness, PenStyle style, bool isVisible)
    {
        var handle = isVisible && style != PenStyle.PS_NULL
            ? EnsurePen(color, thickness, style)
            : EnsureNullPen();
        Document.AddSelectObject(handle);
    }

    private int EnsureBrush(Color color)
    {
        var key = (uint)color.ToArgb();
        if (_brushHandles.TryGetValue(key, out var handle))
        {
            return handle;
        }

        handle = AddBrush(color, BrushStyle.BS_SOLID);
        _brushHandles[key] = handle;
        return handle;
    }

    private int EnsurePen(Color color, double thickness, PenStyle style)
    {
        var width = Math.Max(1, (int)Math.Round(thickness));
        var key = new PenKey((uint)color.ToArgb(), width, style);
        if (_penHandles.TryGetValue(key, out var handle))
        {
            return handle;
        }

        handle = AddPen(color, width, style);
        _penHandles[key] = handle;
        return handle;
    }

    private int EnsureNullBrush()
    {
        if (_nullBrushHandle >= 0)
        {
            return _nullBrushHandle;
        }

        _nullBrushHandle = AddBrush(Color.White, BrushStyle.BS_NULL);
        return _nullBrushHandle;
    }

    private int EnsureNullPen()
    {
        if (_nullPenHandle >= 0)
        {
            return _nullPenHandle;
        }

        _nullPenHandle = AddPen(Color.White, 0, PenStyle.PS_NULL);
        return _nullPenHandle;
    }

    private int AddBrush(Color color, BrushStyle style)
    {
        var handle = _nextHandle++;
        Document.AddCreateBrushIndirect(color, style);
        _handleOrder.Add(handle);
        return handle;
    }

    private int AddPen(Color color, int width, PenStyle style)
    {
        var handle = _nextHandle++;
        Document.AddCreatePenIndirect(color, style, width);
        _handleOrder.Add(handle);
        return handle;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        for (var i = _handleOrder.Count - 1; i >= 0; i--)
        {
            Document.AddDeleteObject(_handleOrder[i]);
        }

        _handleOrder.Clear();
        _brushHandles.Clear();
        _penHandles.Clear();
        _disposed = true;
    }

    private readonly record struct PenKey(uint Color, int Width, PenStyle Style);
}
