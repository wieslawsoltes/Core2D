// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Core2D.Model.Editor;
using Core2D.ViewModels.Editor;

namespace Core2D.Controls.Studio;

/// <summary>Screen-space guide overlay. It does not participate in canvas hit testing.</summary>
public sealed class StudioCanvasOverlay : Control
{
    /// <summary>Defines the themeable guide color.</summary>
    public static readonly StyledProperty<IBrush?> GuideBrushProperty =
        AvaloniaProperty.Register<StudioCanvasOverlay, IBrush?>(nameof(GuideBrush), Brushes.OrangeRed);
    /// <summary>Defines the viewport-local navigation model.</summary>
    public static readonly DirectProperty<StudioCanvasOverlay, CanvasNavigationViewModel?> NavigationProperty =
        AvaloniaProperty.RegisterDirect<StudioCanvasOverlay, CanvasNavigationViewModel?>(nameof(Navigation), x => x.Navigation);
    private CanvasNavigationViewModel? _navigation;
    private CanvasGuidesViewModel? _guides;
    private Matrix _worldToScreen = Matrix.Identity;
    private CanvasGuide? _preview;
    private Guid? _selected;
    private bool _showGuides = true;

    static StudioCanvasOverlay() => AffectsRender<StudioCanvasOverlay>(GuideBrushProperty);

    /// <summary>Raised when page or guide state invalidates an in-flight gesture.</summary>
    public event EventHandler? InteractionInvalidated;

    /// <summary>Gets or sets the guide stroke.</summary>
    public IBrush? GuideBrush { get => GetValue(GuideBrushProperty); set => SetValue(GuideBrushProperty, value); }
    /// <summary>Gets the viewport-local controls model.</summary>
    public CanvasNavigationViewModel? Navigation => _navigation;
    /// <summary>Gets the page-to-overlay affine transform.</summary>
    public Matrix WorldToScreen => _worldToScreen;
    /// <summary>Gets the guide model for this viewport.</summary>
    public CanvasGuidesViewModel? Guides => _guides;
    /// <summary>Gets or sets the currently selected guide identity.</summary>
    public Guid? SelectedGuide { get => _selected; set { _selected = value; InvalidateVisual(); } }
    /// <summary>Gets or sets a transient guide preview; never mutates document history.</summary>
    public CanvasGuide? Preview { get => _preview; set { _preview = value; InvalidateVisual(); } }
    /// <summary>Gets or sets whether guides are drawn along with the rulers.</summary>
    public bool ShowGuides
    {
        get => _showGuides;
        set
        {
            if (_showGuides == value) return;
            _showGuides = value;
            if (!value) InvalidateInteraction();
            InvalidateVisual();
        }
    }

    /// <summary>Attaches display state. Passing null releases page-guide subscriptions.</summary>
    public void Bind(CanvasNavigationViewModel? navigation, CanvasGuidesViewModel? guides)
    {
        if (_guides is not null)
        {
            ((INotifyCollectionChanged)_guides.Items).CollectionChanged -= OnGuidesChanged;
            _guides.PropertyChanged -= OnGuideOptionsChanged;
        }
        _guides = guides;
        SetAndRaise(NavigationProperty, ref _navigation, navigation);
        if (_guides is not null)
        {
            ((INotifyCollectionChanged)_guides.Items).CollectionChanged += OnGuidesChanged;
            _guides.PropertyChanged += OnGuideOptionsChanged;
        }
        _preview = null;
        _selected = null;
        InteractionInvalidated?.Invoke(this, EventArgs.Empty);
        InvalidateVisual();
    }
    /// <summary>Updates mapping using the actual container transform, including layout centering.</summary>
    public void SetTransform(Matrix matrix)
    {
        _worldToScreen = matrix;
        InvalidateVisual();
    }
    /// <summary>Finds the nearest guide within a screen-space tolerance, independently of zoom.</summary>
    public CanvasGuide? HitGuide(Point point, double tolerance = 5)
    {
        if (!ShowGuides || _guides is not { IsVisible: true, IsLocked: false }) return null;
        CanvasGuide? nearest = null;
        foreach (CanvasGuide guide in _guides.Items)
        {
            Point location = new Point(guide.IsVertical ? guide.Position : 0, guide.IsVertical ? 0 : guide.Position) * _worldToScreen;
            double distance = Math.Abs(guide.IsVertical ? point.X - location.X : point.Y - location.Y);
            if (distance <= tolerance) { tolerance = distance; nearest = guide; }
        }
        return nearest;
    }
    /// <summary>Converts overlay-local coordinates to page coordinates.</summary>
    public Point? ToWorld(Point point) => _worldToScreen.TryInvert(out Matrix inverse) ? point * inverse : null;
    private void InvalidateInteraction()
    {
        _preview = null;
        _selected = null;
        InteractionInvalidated?.Invoke(this, EventArgs.Empty);
    }
    private void OnGuidesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Undo or another viewport wins over an uncommitted drag.
        if (_preview is not null) InvalidateInteraction();
        InvalidateVisual();
    }
    private void OnGuideOptionsChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_guides is not { IsVisible: true, IsLocked: false }) InvalidateInteraction();
        InvalidateVisual();
    }
    /// <inheritdoc />
    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (!ShowGuides || _guides is not { IsVisible: true } || GuideBrush is null) return;
        var pen = new Pen(GuideBrush, 1);
        using (context.PushClip(new Rect(Bounds.Size)))
        {
            foreach (CanvasGuide guide in _guides.Items)
                if (_preview?.Id != guide.Id) DrawGuide(context, pen, guide);
            if (_preview is not null) DrawGuide(context, pen, _preview);
        }
    }
    private void DrawGuide(DrawingContext context, Pen pen, CanvasGuide guide)
    {
        Point location = new Point(guide.IsVertical ? guide.Position : 0, guide.IsVertical ? 0 : guide.Position) * _worldToScreen;
        double position = guide.IsVertical ? location.X : location.Y;
        if (!double.IsFinite(position)) return;
        position = Math.Floor(position) + .5;
        Point start = guide.IsVertical ? new Point(position, 0) : new Point(0, position);
        Point end = guide.IsVertical ? new Point(position, Bounds.Height) : new Point(Bounds.Width, position);
        context.DrawLine(pen, start, end);
        if (_selected == guide.Id || _preview?.Id == guide.Id)
            context.DrawEllipse(pen.Brush, null, guide.IsVertical ? new Point(position, 8) : new Point(8, position), 3, 3);
    }
}
