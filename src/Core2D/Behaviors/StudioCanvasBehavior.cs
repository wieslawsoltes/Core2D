// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls;
using Core2D.Controls.Studio;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Layout;
using Core2D.ViewModels.Shapes;

namespace Core2D.Behaviors;

/// <summary>Connects one canvas to its navigation model, actual transform and selection measurements.</summary>
public sealed class StudioCanvasBehavior : Behavior<Control>
{
    private ZoomBorder? _zoom;
    private Control? _container, _corner;
    private Ruler? _horizontal, _vertical;
    private StudioCanvasOverlay? _overlay;
    private ProjectEditorViewModel? _editor;
    private ProjectContainerViewModel? _project;
    private CanvasNavigationViewModel? _navigation;
    private CompositeDisposable? _selectionSubscriptions;
    private Window? _window;
    private Rect? _selection;
    private IPointer? _pointer;
    private Point _panPoint;
    private bool _space, _queued;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } root) return;
        root.Loaded += OnLoaded;
        root.DataContextChanged += OnContextChanged;
        root.LayoutUpdated += OnLayoutUpdated;
        root.AddHandler(InputElement.PointerPressedEvent, OnPressed, RoutingStrategies.Tunnel);
        root.AddHandler(InputElement.PointerMovedEvent, OnMoved, RoutingStrategies.Tunnel);
        root.AddHandler(InputElement.PointerReleasedEvent, OnReleased, RoutingStrategies.Tunnel);
        root.AddHandler(InputElement.PointerWheelChangedEvent, OnWheel, RoutingStrategies.Tunnel);
        root.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        root.AddHandler(InputElement.KeyUpEvent, OnKeyUp, RoutingStrategies.Tunnel);
        root.PointerCaptureLost += OnCaptureLost;
        Connect();
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } root)
        {
            root.Loaded -= OnLoaded;
            root.DataContextChanged -= OnContextChanged;
            root.LayoutUpdated -= OnLayoutUpdated;
            root.RemoveHandler(InputElement.PointerPressedEvent, OnPressed);
            root.RemoveHandler(InputElement.PointerMovedEvent, OnMoved);
            root.RemoveHandler(InputElement.PointerReleasedEvent, OnReleased);
            root.RemoveHandler(InputElement.PointerWheelChangedEvent, OnWheel);
            root.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            root.RemoveHandler(InputElement.KeyUpEvent, OnKeyUp);
            root.PointerCaptureLost -= OnCaptureLost;
        }
        Disconnect();
        base.OnDetaching();
    }
    private void OnLoaded(object? sender, RoutedEventArgs e) { if (_navigation is null) Connect(); UpdateTransform(); }
    private void OnContextChanged(object? sender, EventArgs e) => Connect();
    private void Connect()
    {
        Disconnect();
        if (AssociatedObject is not { } root || root.GetVisualRoot() is null) return;
        _zoom = root.FindControl<ZoomBorder>("PageZoomBorder");
        _container = root.FindControl<Control>("ContainerPanel");
        _overlay = root.FindControl<StudioCanvasOverlay>("CanvasOverlay");
        _horizontal = root.FindControl<Ruler>("HorizontalRuler");
        _vertical = root.FindControl<Ruler>("VerticalRuler");
        _corner = root.FindControl<Control>("RulerCorner");
        _editor = root.DataContext as ProjectEditorViewModel;
        if (_zoom is null || _container is null || _overlay is null || _editor is null) return;
        _navigation = new CanvasNavigationViewModel(ZoomCenter, FitPage, FitSelection);
        _navigation.PropertyChanged += OnNavigationChanged;
        _zoom.ZoomChanged += OnZoomChanged;
        _zoom.PointerExited += OnPointerExited;
        _editor.PropertyChanged += OnEditorChanged;
        _window = TopLevel.GetTopLevel(root) as Window;
        if (_window is not null) _window.Deactivated += OnDeactivated;
        RebindProject();
    }
    private void Disconnect()
    {
        EndPan();
        _space = false;
        if (_window is not null) _window.Deactivated -= OnDeactivated;
        _window = null;
        if (_zoom is not null) { _zoom.ZoomChanged -= OnZoomChanged; _zoom.PointerExited -= OnPointerExited; }
        if (_editor is not null) _editor.PropertyChanged -= OnEditorChanged;
        if (_project is not null) _project.PropertyChanged -= OnProjectChanged;
        _selectionSubscriptions?.Dispose();
        _selectionSubscriptions = null;
        if (_navigation is not null) { _navigation.PropertyChanged -= OnNavigationChanged; _navigation.Dispose(); }
        _overlay?.Bind(null, null);
        _navigation = null;
        _project = null;
        _editor = null;
        _zoom = null;
        _selection = null;
    }
    private void OnEditorChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProjectEditorViewModel.Project)) RebindProject();
    }
    private void RebindProject()
    {
        if (_project is not null) _project.PropertyChanged -= OnProjectChanged;
        _project = _editor?.Project;
        if (_project is not null) _project.PropertyChanged += OnProjectChanged;
        RebindPage();
    }
    private void OnProjectChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProjectContainerViewModel.CurrentContainer)) RebindPage();
        else if (e.PropertyName == nameof(ProjectContainerViewModel.SelectedShapes)) ObserveSelection();
    }
    private void RebindPage()
    {
        EndPan();
        if (_navigation is null) return;
        _navigation.Guides = _project?.CurrentContainer is { } frame ? _editor?.GetCanvasGuides(frame) : null;
        _overlay?.Bind(_navigation, _navigation.Guides);
        ObserveSelection();
        UpdateTransform();
    }
    private void ObserveSelection()
    {
        _selectionSubscriptions?.Dispose();
        _selectionSubscriptions = new CompositeDisposable();
        if (_project?.SelectedShapes is { } shapes)
        {
            foreach (BaseShapeViewModel shape in shapes)
                _selectionSubscriptions.Add(shape.Subscribe(Observer.Create<(object? sender, PropertyChangedEventArgs e)>(_ => QueueSelection())));
        }
        UpdateSelection();
    }
    private void QueueSelection()
    {
        if (_queued) return;
        _queued = true;
        Dispatcher.UIThread.Post(() => { _queued = false; if (_navigation is not null) UpdateSelection(); }, DispatcherPriority.Render);
    }
    private void UpdateSelection()
    {
        _selection = null;
        if (_project?.SelectedShapes is { Count: > 0 } shapes)
        {
            try
            {
                var box = new GroupBox(shapes.ToList()).Bounds;
                _selection = new Rect((double)box.Left, (double)box.Top, Math.Max(0, (double)box.Width), Math.Max(0, (double)box.Height));
            }
            catch (OverflowException) { /* Non-finite imported geometry has no useful ruler measurement. */ }
        }
        if (_navigation is not null)
        {
            _navigation.HasSelection = _selection is not null;
            _navigation.SelectionSummary = _selection is { } rect ? $"{rect.Width:0.##} × {rect.Height:0.##} px" : string.Empty;
        }
        if (_horizontal is not null) { _horizontal.SelectionStart = _selection?.X ?? 0; _horizontal.SelectionLength = _selection?.Width ?? 0; }
        if (_vertical is not null) { _vertical.SelectionStart = _selection?.Y ?? 0; _vertical.SelectionLength = _selection?.Height ?? 0; }
    }
    private void OnZoomChanged(object? sender, ZoomChangedEventArgs e) => UpdateTransform();
    private void OnLayoutUpdated(object? sender, EventArgs e) => UpdateTransform();
    private void UpdateTransform()
    {
        if (_container is null || _overlay is null || _zoom is null || _navigation is null) return;
        if (_container.TransformToVisual(_overlay) is not { } matrix || !double.IsFinite(matrix.M11)
            || !double.IsFinite(matrix.M22) || matrix.M11 <= 0 || matrix.M22 <= 0) return;
        if (_overlay.WorldToScreen != matrix) _overlay.SetTransform(matrix);
        _navigation.UpdateZoom(_zoom.ZoomX);
        if (_horizontal is not null)
        {
            _horizontal.Zoom = matrix.M11; _horizontal.Offset = matrix.M31;
            _horizontal.HighlightStart = 0; _horizontal.HighlightLength = _container.Bounds.Width;
        }
        if (_vertical is not null)
        {
            _vertical.Zoom = matrix.M22; _vertical.Offset = matrix.M32;
            _vertical.HighlightStart = 0; _vertical.HighlightLength = _container.Bounds.Height;
        }
    }
    private void OnNavigationChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_navigation is null) return;
        if (e.PropertyName == nameof(CanvasNavigationViewModel.ShowRulers))
        {
            if (_horizontal is not null) _horizontal.IsVisible = _navigation.ShowRulers;
            if (_vertical is not null) _vertical.IsVisible = _navigation.ShowRulers;
            if (_corner is not null) _corner.IsVisible = _navigation.ShowRulers;
            if (_overlay is not null) _overlay.ShowGuides = _navigation.ShowRulers;
        }
        if (e.PropertyName == nameof(CanvasNavigationViewModel.IsHandTool)) { EndPan(); UpdateCursor(); }
    }
    private void ZoomCenter(double value) => ZoomAt(value, _zoom is null ? default : new Point(_zoom.Bounds.Width / 2, _zoom.Bounds.Height / 2));
    private void ZoomAt(double value, Point viewportPoint)
    {
        if (_zoom?.Child is not { } child || !_zoom.EnableZoom || _zoom.ZoomX <= 0) return;
        double minimum = Math.Max(.01, _zoom.MinZoomX), maximum = Math.Min(256, _zoom.MaxZoomX);
        if (!double.IsFinite(value) || minimum > maximum) return;
        value = Math.Clamp(value, minimum, maximum);
        if (_zoom.TranslatePoint(viewportPoint, child) is { } center)
            _zoom.ZoomTo(value / _zoom.ZoomX, center.X, center.Y, true);
        UpdateTransform();
    }
    private void FitPage()
    {
        if (_container is not null) Fit(new Rect(_container.Bounds.Size));
    }
    private void FitSelection() { UpdateSelection(); if (_selection is { } rect) Fit(rect); }
    private void Fit(Rect rect)
    {
        if (_zoom is null || _container is null || rect.Width < 0 || rect.Height < 0) return;
        double width = Math.Max(1, _zoom.Bounds.Width - 80), height = Math.Max(1, _zoom.Bounds.Height - 120);
        ZoomCenter(Math.Min(width / Math.Max(1, rect.Width), height / Math.Max(1, rect.Height)));
        if (_container.TranslatePoint(rect.Center, _zoom) is { } position)
            _zoom.PanDelta(_zoom.Bounds.Width / 2 - position.X, (_zoom.Bounds.Height - 40) / 2 - position.Y, true);
        UpdateTransform();
    }
    private bool CanvasSource(object? source)
    {
        for (Visual? visual = source as Visual; visual is not null; visual = visual.GetVisualParent())
        {
            if (visual is TextBox) return false;
            if (ReferenceEquals(visual, _zoom)) return true;
        }
        return false;
    }
    private void OnPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_zoom is null || !CanvasSource(e.Source) || (!_space && _navigation?.IsHandTool != true)
            || _editor?.IsToolIdle != true || !_zoom.EnablePan || !e.GetCurrentPoint(_zoom).Properties.IsLeftButtonPressed) return;
        _panPoint = e.GetPosition(AssociatedObject);
        _pointer = e.Pointer;
        _pointer.Capture(AssociatedObject);
        _zoom.Focus();
        UpdateCursor();
        e.Handled = true;
    }
    private void OnMoved(object? sender, PointerEventArgs e)
    {
        if (_pointer == e.Pointer && _zoom is not null)
        {
            Point point = e.GetPosition(AssociatedObject);
            Vector delta = point - _panPoint;
            _panPoint = point;
            _zoom.PanDelta(delta.X, delta.Y, true);
            e.Handled = true;
            return;
        }
        if (_overlay is null || !CanvasSource(e.Source) || _overlay.ToWorld(e.GetPosition(_overlay)) is not { } world) return;
        if (_horizontal is not null) _horizontal.Marker = world.X;
        if (_vertical is not null) _vertical.Marker = world.Y;
        if (_navigation is not null) _navigation.PointerSummary = $"X {world.X:0.##}   Y {world.Y:0.##}";
        // Hand mode prevents hover/tool previews from modifying the drawing while navigating.
        if (_space || _navigation?.IsHandTool == true) e.Handled = true;
    }
    private void OnReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_pointer != e.Pointer) return;
        EndPan();
        e.Handled = true;
    }
    private void OnWheel(object? sender, PointerWheelEventArgs e)
    {
        if (_zoom is null || !CanvasSource(e.Source)) return;
        if ((e.KeyModifiers & (KeyModifiers.Control | KeyModifiers.Meta)) != 0)
            ZoomAt(_zoom.ZoomX * Math.Pow(1.1, Math.Clamp(e.Delta.Y, -20, 20)), e.GetPosition(_zoom));
        else if (_zoom.EnablePan)
        {
            Vector delta = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? new Vector(e.Delta.Y, e.Delta.X) : e.Delta;
            _zoom.PanDelta(delta.X * 32, delta.Y * 32, true);
        }
        e.Handled = true;
    }
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (!CanvasSource(e.Source) || _navigation is null) return;
        if (e.Key == Key.Space && e.KeyModifiers == KeyModifiers.None && _editor?.IsToolIdle == true)
        { _space = true; UpdateCursor(); e.Handled = true; }
        else if (e.Key == Key.Escape && (_space || _pointer is not null))
        { _space = false; EndPan(); e.Handled = true; }
        else if (e.KeyModifiers == KeyModifiers.Shift)
        {
            if (e.Key == Key.D1) { FitPage(); e.Handled = true; }
            else if (e.Key == Key.D2) { FitSelection(); e.Handled = true; }
            else if (e.Key == Key.R) { _navigation.ShowRulers = !_navigation.ShowRulers; e.Handled = true; }
        }
    }
    private void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Space && _space) { _space = false; UpdateCursor(); e.Handled = true; }
    }
    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        if (_horizontal is not null) _horizontal.Marker = null;
        if (_vertical is not null) _vertical.Marker = null;
        if (_navigation is not null) _navigation.PointerSummary = string.Empty;
    }
    private void OnCaptureLost(object? sender, PointerCaptureLostEventArgs e) => EndPan();
    private void OnDeactivated(object? sender, EventArgs e) { _space = false; EndPan(); }
    private void EndPan() { IPointer? pointer = _pointer; _pointer = null; pointer?.Capture(null); UpdateCursor(); }
    private void UpdateCursor()
    {
        if (_zoom is not null) _zoom.Cursor = _pointer is not null ? new Cursor(StandardCursorType.SizeAll)
            : _space || _navigation?.IsHandTool == true ? new Cursor(StandardCursorType.Hand) : null;
    }
}
