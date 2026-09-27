// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Globalization;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Keyboard increments and transactional prefix scrubbing.</summary>
public sealed class StudioNumberInputBehavior : Behavior<StudioNumberBox>
{
    private ContentPresenter? _handle;
    private IPointer? _pointer;
    private double _startX;
    private double _origin;
    private double _candidate;
    private bool _moved;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is { } field)
        {
            field.TemplateApplied += OnTemplateApplied;
            field.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
            AttachHandle(field.GetVisualDescendants().OfType<ContentPresenter>().FirstOrDefault(x =>
                x.Name == "PART_InnerLeft" && ReferenceEquals(x.TemplatedParent, field)));
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } field)
        {
            field.TemplateApplied -= OnTemplateApplied;
            field.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
        }
        DetachHandle();
        base.OnDetaching();
    }

    private void OnTemplateApplied(object? sender, TemplateAppliedEventArgs e) =>
        AttachHandle(e.NameScope.Find<ContentPresenter>("PART_InnerLeft"));

    private void AttachHandle(ContentPresenter? presenter)
    {
        DetachHandle();
        _handle = presenter;
        if (_handle is { } handle)
        {
            handle.PointerPressed += OnPointerPressed;
            handle.PointerMoved += OnPointerMoved;
            handle.PointerReleased += OnPointerReleased;
            handle.PointerCaptureLost += OnCaptureLost;
        }
    }

    private void DetachHandle()
    {
        CancelScrub();
        if (_handle is { } handle)
        {
            handle.PointerPressed -= OnPointerPressed;
            handle.PointerMoved -= OnPointerMoved;
            handle.PointerReleased -= OnPointerReleased;
            handle.PointerCaptureLost -= OnCaptureLost;
            _handle = null;
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_pointer is not null && e.Key == Key.Escape)
        {
            CancelScrub();
            e.Handled = true;
            return;
        }
        if (AssociatedObject is not { IsReadOnly: false, AcceptsReturn: false } field
            || e.Key is not (Key.Up or Key.Down)
            || (e.KeyModifiers & (KeyModifiers.Control | KeyModifiers.Meta)) != 0
            || !TryRead(field, out var value))
        {
            return;
        }
        Write(field, value + (e.Key == Key.Up ? 1 : -1) * Increment(field, e.KeyModifiers));
        e.Handled = true;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (AssociatedObject is not { IsReadOnly: false, IsEffectivelyEnabled: true } field
            || !e.GetCurrentPoint(_handle).Properties.IsLeftButtonPressed || !TryRead(field, out _origin))
        {
            return;
        }
        _candidate = _origin;
        _startX = e.GetPosition(field).X;
        _moved = false;
        _pointer = e.Pointer;
        _pointer.Capture(_handle);
        field.Focus();
        e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_pointer != e.Pointer || AssociatedObject is not { } field)
        {
            return;
        }
        var delta = e.GetPosition(field).X - _startX;
        _moved |= Math.Abs(delta) >= 3;
        _candidate = _origin + Math.Truncate(delta / 3) * Increment(field, e.KeyModifiers);
        if (_moved && _handle is { } handle && double.IsFinite(_candidate))
        {
            // Release commits once. Escape, capture loss and detachment commit nothing.
            ToolTip.SetTip(handle, _candidate.ToString("G12", CultureInfo.CurrentCulture));
            ToolTip.SetIsOpen(handle, true);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_pointer != e.Pointer || AssociatedObject is not { } field)
        {
            return;
        }
        if (_moved)
        {
            Write(field, _candidate);
        }
        else
        {
            field.SelectAll();
        }
        CancelScrub();
        e.Handled = true;
    }

    private void OnCaptureLost(object? sender, PointerCaptureLostEventArgs e) => CancelScrub();

    private void CancelScrub()
    {
        var pointer = _pointer;
        _pointer = null;
        if (_handle is { } handle)
        {
            ToolTip.SetIsOpen(handle, false);
        }
        pointer?.Capture(null);
    }

    private static bool TryRead(TextBox field, out double value) =>
        double.TryParse(field.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out value) && double.IsFinite(value);

    private static double Increment(StudioNumberBox field, KeyModifiers modifiers) => field.Step
        * (modifiers.HasFlag(KeyModifiers.Shift) ? 10 : modifiers.HasFlag(KeyModifiers.Alt) ? 0.1 : 1);

    private static void Write(TextBox field, double value)
    {
        if (double.IsFinite(value))
        {
            field.SetCurrentValue(TextBox.TextProperty, value.ToString("G12", CultureInfo.CurrentCulture));
        }
    }
}
