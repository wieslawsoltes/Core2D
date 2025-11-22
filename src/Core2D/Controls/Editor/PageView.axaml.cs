// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia.Controls;
using Avalonia;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Reactive;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Renderer;
using System;
using System.ComponentModel;

namespace Core2D.Controls.Editor;

public partial class PageView : UserControl
{
    private IDisposable? _boundsSubscription;
    private IDisposable? _stateSubscription;

    public PageView()
    {
        InitializeComponent();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Unsubscribe();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        Subscribe();
        UpdateRulers();
    }

    private void Subscribe()
    {
        Unsubscribe();

        if (ContainerPanel is { })
        {
            _boundsSubscription = ContainerPanel.GetPropertyChangedObservable(BoundsProperty)
                .Subscribe(_ => UpdateRulers());
        }

        if (PageZoomBorder is { })
        {
            PageZoomBorder.ZoomChanged += ZoomChanged;
        }

        var state = GetRendererState();
        if (state is { })
        {
            state.PropertyChanged += StateOnPropertyChanged;
            _stateSubscription = new ActionDisposable(() => state.PropertyChanged -= StateOnPropertyChanged);
        }
    }

    private void Unsubscribe()
    {
        _boundsSubscription?.Dispose();
        _boundsSubscription = null;

        if (PageZoomBorder is { })
        {
            PageZoomBorder.ZoomChanged -= ZoomChanged;
        }

        _stateSubscription?.Dispose();
        _stateSubscription = null;
    }

    private void ZoomChanged(object? sender, ZoomChangedEventArgs e)
    {
        UpdateRulers();
    }

    private void StateOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ShapeRendererStateViewModel.RulerSelectionLeft)
            or nameof(ShapeRendererStateViewModel.RulerSelectionTop)
            or nameof(ShapeRendererStateViewModel.RulerSelectionWidth)
            or nameof(ShapeRendererStateViewModel.RulerSelectionHeight)
            or nameof(ShapeRendererStateViewModel.RulerHasSelection))
        {
            UpdateRulers();
        }
    }

    private ShapeRendererStateViewModel? GetRendererState()
    {
        return (DataContext as ProjectEditorViewModel)?.Renderer?.State;
    }

    private void UpdateRulers()
    {
        if (PageZoomBorder is not { } zoomBorder || ContainerPanel is null || HorizontalRuler is null || VerticalRuler is null)
        {
            return;
        }

        var zoomX = zoomBorder.ZoomX;
        var zoomY = zoomBorder.ZoomY;
        var effectiveOffsetX = zoomBorder.OffsetX;
        var effectiveOffsetY = zoomBorder.OffsetY;
        var effectiveZoomX = zoomX;
        var effectiveZoomY = zoomY;
        var bounds = ContainerPanel.Bounds;

        if (zoomX <= 0 || zoomY <= 0)
        {
            return;
        }

        var transform = ContainerPanel.TransformToVisual(zoomBorder);
        if (transform is { })
        {
            var matrix = transform.Value;

            if (Math.Abs(matrix.M11) > double.Epsilon)
            {
                effectiveZoomX = matrix.M11;
            }

            if (Math.Abs(matrix.M22) > double.Epsilon)
            {
                effectiveZoomY = matrix.M22;
            }

            effectiveOffsetX = matrix.M31;
            effectiveOffsetY = matrix.M32;
        }

        var topLeft = ContainerPanel.TranslatePoint(new Point(0, 0), zoomBorder);
        var leftWorld = topLeft is { } tl ? (tl.X - effectiveOffsetX) / effectiveZoomX : (bounds.X - effectiveOffsetX) / effectiveZoomX;
        var topWorld = topLeft is { } tp ? (tp.Y - effectiveOffsetY) / effectiveZoomY : (bounds.Y - effectiveOffsetY) / effectiveZoomY;

        HorizontalRuler.Zoom = effectiveZoomX;
        HorizontalRuler.Offset = effectiveOffsetX;
        HorizontalRuler.HighlightStart = leftWorld;
        HorizontalRuler.HighlightLength = bounds.Width;
        VerticalRuler.Zoom = effectiveZoomY;
        VerticalRuler.Offset = effectiveOffsetY;
        VerticalRuler.HighlightStart = topWorld;
        VerticalRuler.HighlightLength = bounds.Height;

        var state = GetRendererState();
        if (state is { RulerHasSelection: true })
        {
            HorizontalRuler.SelectionStart = state.RulerSelectionLeft;
            HorizontalRuler.SelectionLength = state.RulerSelectionWidth;
            VerticalRuler.SelectionStart = state.RulerSelectionTop;
            VerticalRuler.SelectionLength = state.RulerSelectionHeight;

            state.RulerPageLeft = leftWorld;
            state.RulerPageTop = topWorld;
            state.RulerPageWidth = HorizontalRuler.HighlightLength;
            state.RulerPageHeight = VerticalRuler.HighlightLength;
        }
        else
        {
            HorizontalRuler.SelectionStart = 0;
            HorizontalRuler.SelectionLength = 0;
            VerticalRuler.SelectionStart = 0;
            VerticalRuler.SelectionLength = 0;

            if (state is { })
            {
                state.RulerPageLeft = leftWorld;
                state.RulerPageTop = topWorld;
                state.RulerPageWidth = HorizontalRuler.HighlightLength;
                state.RulerPageHeight = VerticalRuler.HighlightLength;
            }
        }
    }

    private sealed class ActionDisposable : IDisposable
    {
        private Action? _dispose;

        public ActionDisposable(Action dispose)
        {
            _dispose = dispose;
        }

        public void Dispose()
        {
            _dispose?.Invoke();
            _dispose = null;
        }
    }
}
