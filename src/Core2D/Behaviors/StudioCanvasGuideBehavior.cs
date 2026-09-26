// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;
using Core2D.Model.Editor;
using Core2D.ViewModels.Editor;

namespace Core2D.Behaviors;

/// <summary>Ruler-guide gestures with screen-space hit testing and commit-on-release history.</summary>
public sealed class StudioCanvasGuideBehavior : Behavior<Control>
{
    private StudioCanvasOverlay? _overlay;
    private Control? _horizontal, _vertical;
    private IPointer? _pointer;
    private CanvasGuide? _original;
    private bool _copy;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } root) return;
        root.Loaded += OnLoaded;
        root.AddHandler(InputElement.PointerPressedEvent, OnPressed, RoutingStrategies.Tunnel);
        root.AddHandler(InputElement.PointerMovedEvent, OnMoved, RoutingStrategies.Tunnel);
        root.AddHandler(InputElement.PointerReleasedEvent, OnReleased, RoutingStrategies.Tunnel);
        root.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        root.PointerCaptureLost += OnCaptureLost;
        Connect();
    }
    private void OnLoaded(object? sender, RoutedEventArgs e) => Connect();
    private void Connect()
    {
        if (_overlay is not null) _overlay.SessionChanged -= OnSessionChanged;
        _overlay = AssociatedObject?.FindControl<StudioCanvasOverlay>("CanvasOverlay");
        if (_overlay is not null) _overlay.SessionChanged += OnSessionChanged;
        _horizontal = AssociatedObject?.FindControl<Control>("HorizontalRuler");
        _vertical = AssociatedObject?.FindControl<Control>("VerticalRuler");
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        Cancel();
        if (AssociatedObject is { } root)
        {
            root.Loaded -= OnLoaded;
            root.RemoveHandler(InputElement.PointerPressedEvent, OnPressed);
            root.RemoveHandler(InputElement.PointerMovedEvent, OnMoved);
            root.RemoveHandler(InputElement.PointerReleasedEvent, OnReleased);
            root.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            root.PointerCaptureLost -= OnCaptureLost;
        }
        if (_overlay is not null) _overlay.SessionChanged -= OnSessionChanged;
        _overlay = null;
        _horizontal = _vertical = null;
        base.OnDetaching();
    }
    private static bool Inside(Control? control, PointerEventArgs e) =>
        control is { IsEffectivelyVisible: true } && new Rect(control.Bounds.Size).Contains(e.GetPosition(control));
    private void OnPressed(object? sender, PointerPressedEventArgs e)
    {
        if (_overlay?.Guides is not { IsVisible: true, IsLocked: false } || !_overlay.ShowGuides
            || !e.GetCurrentPoint(AssociatedObject).Properties.IsLeftButtonPressed
            || AssociatedObject?.DataContext is not ProjectEditorViewModel { IsToolIdle: true }) return;
        if (_overlay.Navigation?.IsHandTool == true) return;
        bool fromHorizontal = Inside(_horizontal, e), fromVertical = Inside(_vertical, e);
        Point point = e.GetPosition(_overlay);
        CanvasGuide? hit = Inside(_overlay, e) ? _overlay.HitGuide(point) : null;
        if (!fromHorizontal && !fromVertical && hit is null)
        {
            _overlay.SelectedGuide = null;
            return;
        }
        if (_overlay.ToWorld(point) is not { } world) return;
        _original = hit;
        _copy = hit is not null && e.KeyModifiers.HasFlag(KeyModifiers.Alt);
        bool vertical = hit?.IsVertical ?? fromVertical;
        var preview = new CanvasGuide(_copy || hit is null ? Guid.NewGuid() : hit.Id, vertical, vertical ? world.X : world.Y);
        _overlay.Preview = preview;
        _overlay.SelectedGuide = preview.Id;
        _pointer = e.Pointer;
        _pointer.Capture(AssociatedObject);
        AssociatedObject?.FindControl<Control>("PageZoomBorder")?.Focus();
        e.Handled = true;
    }
    private void OnMoved(object? sender, PointerEventArgs e)
    {
        if (_pointer != e.Pointer || _overlay?.Preview is not { } preview) return;
        if (_overlay.Guides is not { IsVisible: true, IsLocked: false } || !_overlay.ShowGuides) { Cancel(); return; }
        if (_overlay.ToWorld(e.GetPosition(_overlay)) is not { } world) return;
        double value = preview.IsVertical ? world.X : world.Y;
        if (e.KeyModifiers.HasFlag(KeyModifiers.Shift)) value = Math.Round(value / 10) * 10;
        if (!double.IsFinite(value) || Math.Abs(value) > 1e15) return;
        _overlay.Preview = preview with { Position = value };
        if (_overlay.Navigation is { } navigation) navigation.PointerSummary = $"Guide  {value:0.##} px";
        e.Handled = true;
    }
    private void OnReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_pointer != e.Pointer || _overlay?.Preview is not { } preview) return;
        CanvasGuidesViewModel? guides = _overlay.Guides;
        bool valid = Inside(_overlay, e);
        Guid? selected = null;
        if (valid)
        {
            if (_original is null || _copy) selected = guides?.Add(preview.IsVertical, preview.Position);
            else { guides?.Move(_original.Id, preview.Position); selected = _original.Id; }
        }
        else if (_original is not null && !_copy) guides?.Remove(_original.Id);
        Cancel();
        _overlay.SelectedGuide = selected;
        e.Handled = true;
    }
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_pointer is not null && e.Key == Key.Escape) { Cancel(); e.Handled = true; return; }
        if (e.Source is Visual visual)
            for (Visual? p = visual; p is not null; p = p.GetVisualParent()) if (p is TextBox) return;
        if (_overlay?.SelectedGuide is not { } id || _overlay.Guides is not { IsVisible: true, IsLocked: false } guides) return;
        if (e.Key is Key.Delete or Key.Back)
        {
            guides.Remove(id); _overlay.SelectedGuide = null; e.Handled = true;
        }
        else if (e.Key == Key.Escape) { _overlay.SelectedGuide = null; e.Handled = true; }
        else if (e.Key is Key.Left or Key.Right or Key.Up or Key.Down)
        {
            foreach (CanvasGuide guide in guides.Items)
            {
                if (guide.Id != id) continue;
                if ((guide.IsVertical && e.Key is Key.Left or Key.Right) || (!guide.IsVertical && e.Key is Key.Up or Key.Down))
                {
                    double delta = e.Key is Key.Left or Key.Up ? -1 : 1;
                    guides.Move(id, guide.Position + delta * (e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? 10 : 1));
                }
                e.Handled = true;
                break;
            }
        }
    }
    private void OnSessionChanged(object? sender, EventArgs e) => Cancel();
    private void OnCaptureLost(object? sender, PointerCaptureLostEventArgs e) => Cancel();
    private void Cancel()
    {
        IPointer? pointer = _pointer;
        _pointer = null;
        _original = null;
        if (_overlay is not null) _overlay.Preview = null;
        pointer?.Capture(null);
    }
}
