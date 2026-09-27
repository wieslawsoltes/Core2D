// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Mouse/pen/touch capture and keyboard editing for the HSV plane; Escape restores a gesture's initial value.</summary>
public sealed class StudioColorPlaneBehavior : Behavior<StudioColorPlane>
{
    private IPointer? _pointer;
    private HsvColor _original;
    private bool _writing;
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } plane) return;
        plane.PointerPressed += OnPressed; plane.PointerMoved += OnMoved;
        plane.PointerReleased += OnReleased; plane.PointerCaptureLost += OnCaptureLost;
        plane.KeyDown += OnKeyDown; plane.PropertyChanged += OnChanged;
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        CancelCapture();
        if (AssociatedObject is { } plane)
        {
            plane.PointerPressed -= OnPressed; plane.PointerMoved -= OnMoved;
            plane.PointerReleased -= OnReleased; plane.PointerCaptureLost -= OnCaptureLost;
            plane.KeyDown -= OnKeyDown; plane.PropertyChanged -= OnChanged;
        }
        base.OnDetaching();
    }
    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (!_writing && (e.Property == StudioColorPlane.HsvProperty || e.Property == StyledElement.DataContextProperty
            || e.Property.Name == nameof(InputElement.IsEffectivelyEnabled))) CancelCapture();
    }
    private void OnPressed(object? sender, PointerPressedEventArgs e)
    {
        if (AssociatedObject is not { IsEffectivelyEnabled: true } plane || !e.GetCurrentPoint(plane).Properties.IsLeftButtonPressed) return;
        _original = plane.Hsv; _pointer = e.Pointer; _pointer.Capture(plane); plane.Focus();
        Update(e); e.Handled = true;
    }
    private void OnMoved(object? sender, PointerEventArgs e) { if (_pointer == e.Pointer) { Update(e); e.Handled = true; } }
    private void OnReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_pointer != e.Pointer) return;
        Update(e); CancelCapture(); e.Handled = true;
    }
    private void OnCaptureLost(object? sender, PointerCaptureLostEventArgs e) => CancelCapture();
    private void CancelCapture() { IPointer? pointer = _pointer; _pointer = null; pointer?.Capture(null); }
    private void Update(PointerEventArgs e)
    {
        if (AssociatedObject is not { IsEffectivelyEnabled: true } plane || plane.Bounds.Width <= 0 || plane.Bounds.Height <= 0) return;
        Point point = e.GetPosition(plane);
        Write(new HsvColor(plane.Hsv.A, plane.Hsv.H, System.Math.Clamp(point.X / plane.Bounds.Width, 0, 1), 1 - System.Math.Clamp(point.Y / plane.Bounds.Height, 0, 1)));
    }
    private void Write(HsvColor value)
    {
        _writing = true;
        try { AssociatedObject?.SetCurrentValue(StudioColorPlane.HsvProperty, value); }
        finally { _writing = false; }
    }
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (AssociatedObject is not { IsEffectivelyEnabled: true } plane) return;
        if (e.Key == Key.Escape && _pointer is not null)
        {
            CancelCapture(); Write(_original); e.Handled = true; return;
        }
        if ((e.KeyModifiers & (KeyModifiers.Control | KeyModifiers.Meta | KeyModifiers.Alt)) != 0) return;
        double step = e.KeyModifiers.HasFlag(KeyModifiers.Shift) ? 0.1 : 0.01;
        double s = plane.Hsv.S, v = plane.Hsv.V;
        switch (e.Key)
        {
            case Key.Left: s -= step; break;
            case Key.Right: s += step; break;
            case Key.Up: v += step; break;
            case Key.Down: v -= step; break;
            default: return;
        }
        Write(new HsvColor(plane.Hsv.A, plane.Hsv.H, System.Math.Clamp(s, 0, 1), System.Math.Clamp(v, 0, 1)));
        e.Handled = true;
    }
}
