// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.Views.Renderer;

namespace Core2D.Controls;

public class BlockTileView : Control
{
    public static readonly StyledProperty<BlockShapeViewModel?> BlockProperty =
        AvaloniaProperty.Register<BlockTileView, BlockShapeViewModel?>(nameof(Block));

    public static readonly StyledProperty<IShapeRenderer?> RendererProperty =
        AvaloniaProperty.Register<BlockTileView, IShapeRenderer?>(nameof(Renderer));

    private const double PreviewPadding = 4.0;

    private IDisposable? _groupSubscription;
    private readonly IObserver<(object? sender, System.ComponentModel.PropertyChangedEventArgs e)> _groupObserver;
    private readonly List<PointShapeViewModel> _points = new();

    static BlockTileView()
    {
        AffectsRender<BlockTileView>(BlockProperty, RendererProperty, BoundsProperty);
    }

    public BlockTileView()
    {
        _groupObserver = new InvalidateObserver(this);
        this.GetObservable(BlockProperty).Subscribe(OnBlockChanged);
    }

    public BlockShapeViewModel? Block
    {
        get => GetValue(BlockProperty);
        set => SetValue(BlockProperty, value);
    }

    public IShapeRenderer? Renderer
    {
        get => GetValue(RendererProperty);
        set => SetValue(RendererProperty, value);
    }

    private void OnBlockChanged(BlockShapeViewModel? block)
    {
        _groupSubscription?.Dispose();
        _groupSubscription = block?.Subscribe(_groupObserver);
        InvalidateVisual();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        _groupSubscription?.Dispose();
        _groupSubscription = null;
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var block = Block;
        if (block is null)
        {
            return;
        }

        var renderer = Renderer ?? RendererOptions.GetRenderer(this);
        if (renderer is null)
        {
            return;
        }

        if (Bounds.Width <= 0 || Bounds.Height <= 0)
        {
            return;
        }

        _points.Clear();
        block.GetPoints(_points);

        if (_points.Count == 0)
        {
            return;
        }

        double minX = double.PositiveInfinity;
        double minY = double.PositiveInfinity;
        double maxX = double.NegativeInfinity;
        double maxY = double.NegativeInfinity;

        foreach (var point in _points)
        {
            minX = Math.Min(minX, point.X);
            minY = Math.Min(minY, point.Y);
            maxX = Math.Max(maxX, point.X);
            maxY = Math.Max(maxY, point.Y);
        }

        if (double.IsInfinity(minX) || double.IsInfinity(minY) ||
            double.IsInfinity(maxX) || double.IsInfinity(maxY))
        {
            return;
        }

        var width = maxX - minX;
        var height = maxY - minY;

        if (width <= double.Epsilon)
        {
            width = 1.0;
        }

        if (height <= double.Epsilon)
        {
            height = 1.0;
        }

        var availableWidth = Math.Max(0.0, Bounds.Width - (PreviewPadding * 2.0));
        var availableHeight = Math.Max(0.0, Bounds.Height - (PreviewPadding * 2.0));

        if (availableWidth <= 0.0 || availableHeight <= 0.0)
        {
            return;
        }

        var scale = Math.Min(availableWidth / width, availableHeight / height);
        if (!double.IsFinite(scale) || scale <= 0.0)
        {
            scale = 1.0;
        }

        var offsetX = (Bounds.Width - (width * scale)) / 2.0;
        var offsetY = (Bounds.Height - (height * scale)) / 2.0;

        var clipRect = new Rect(Bounds.Size);

        using var clip = context.PushClip(clipRect);
        using var translateToCenter = context.PushTransform(Matrix.CreateTranslation(offsetX, offsetY));
        using var scaleTransform = context.PushTransform(Matrix.CreateScale(scale, scale));
        using var translateToOrigin = context.PushTransform(Matrix.CreateTranslation(-minX, -minY));

        var state = renderer.State;
        var restoreZoom = false;
        var zoomX = 1.0;
        var zoomY = 1.0;

        if (state is { })
        {
            restoreZoom = true;
            zoomX = state.ZoomX;
            zoomY = state.ZoomY;
            state.ZoomX = scale;
            state.ZoomY = scale;
        }

        try
        {
            block.DrawShape(context, renderer, null);
        }
        finally
        {
            if (restoreZoom && state is { })
            {
                state.ZoomX = zoomX;
                state.ZoomY = zoomY;
            }
        }
    }

    private sealed class InvalidateObserver : IObserver<(object? sender, System.ComponentModel.PropertyChangedEventArgs e)>
    {
        private readonly BlockTileView _owner;

        public InvalidateObserver(BlockTileView owner)
        {
            _owner = owner;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext((object? sender, System.ComponentModel.PropertyChangedEventArgs e) value)
        {
            _owner.InvalidateVisual();
        }
    }
}
