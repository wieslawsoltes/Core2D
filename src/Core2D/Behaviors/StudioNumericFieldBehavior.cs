// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Routes native numeric input into the field's draft/commit transaction.</summary>
public sealed class StudioNumericFieldBehavior : Behavior<StudioNumericField>
{
    private TextBox? _input;
    private Control? _handle;
    private IPointer? _pointer;
    private decimal _origin;
    private decimal _candidate;
    private double _startX;
    private bool _moved;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is { } field)
        {
            field.TemplateApplied += OnTemplateApplied;
            field.PropertyChanged += OnFieldChanged;
            field.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        }
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } field)
        {
            field.TemplateApplied -= OnTemplateApplied;
            field.PropertyChanged -= OnFieldChanged;
            field.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
        }
        DetachParts();
        base.OnDetaching();
    }

    private void OnTemplateApplied(object? sender, TemplateAppliedEventArgs e)
    {
        DetachParts();
        _input = e.NameScope.Find<TextBox>("PART_Input");
        _handle = e.NameScope.Find<Control>("PART_ScrubHandle");
        if (_input is { } input) input.LostFocus += OnLostFocus;
        if (_handle is { } handle)
        {
            handle.PointerPressed += OnPointerPressed;
            handle.PointerMoved += OnPointerMoved;
            handle.PointerReleased += OnPointerReleased;
            handle.PointerCaptureLost += OnCaptureLost;
        }
    }

    private void DetachParts()
    {
        CancelScrub();
        if (_input is { } input) input.LostFocus -= OnLostFocus;
        if (_handle is { } handle)
        {
            handle.PointerPressed -= OnPointerPressed;
            handle.PointerMoved -= OnPointerMoved;
            handle.PointerReleased -= OnPointerReleased;
            handle.PointerCaptureLost -= OnCaptureLost;
        }
        _input = null;
        _handle = null;
    }

    private void OnFieldChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == StudioNumericField.ValueProperty || e.Property == StyledElement.DataContextProperty
            || e.Property == StudioNumericField.IsReadOnlyProperty || e.Property == InputElement.IsEnabledProperty
            || e.Property == StudioNumericField.MinimumProperty || e.Property == StudioNumericField.MaximumProperty)
            CancelScrub();
    }

    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        if (_pointer is null) AssociatedObject?.TryCommit();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (AssociatedObject is not { } field) return;
        // Leave composition commit/cancel keys to the native presenter and input method.
        if (_input?.GetVisualDescendants().OfType<TextPresenter>().Any(x => !string.IsNullOrEmpty(x.PreeditText)) == true) return;
        if (e.Key == Key.Escape)
        {
            CancelScrub();
            field.CancelEdit();
            _input?.SelectAll();
            e.Handled = true;
            return;
        }
        if (field.IsReadOnly || !field.IsEffectivelyEnabled || _pointer is not null) return;
        if (e.Key == Key.Enter)
        {
            if (field.TryCommit()) _input?.SelectAll();
            e.Handled = true;
        }
        else if (e.Key is Key.Up or Key.Down && (e.KeyModifiers & (KeyModifiers.Control | KeyModifiers.Meta)) == 0
                 && field.TryGetCandidate(out decimal value))
        {
            field.CommitValue(AddBounded(value, (e.Key == Key.Up ? 1 : -1) * Step(field, e.KeyModifiers)));
            _input?.SelectAll();
            e.Handled = true;
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (AssociatedObject is not { IsReadOnly: false, IsEffectivelyEnabled: true } field
            || !e.GetCurrentPoint(_handle).Properties.IsLeftButtonPressed || !field.TryGetCandidate(out _origin)) return;
        _candidate = _origin;
        _startX = e.GetPosition(field).X;
        _moved = false;
        _pointer = e.Pointer;
        _pointer.Capture(_handle);
        _input?.Focus();
        e.Handled = true;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_pointer != e.Pointer || AssociatedObject is not { } field) return;
        double delta = e.GetPosition(field).X - _startX;
        if (!double.IsFinite(delta)) return;
        _moved |= Math.Abs(delta) >= 3;
        decimal steps = (decimal)Math.Clamp(Math.Truncate(delta / 3), -1000000, 1000000);
        decimal change;
        try { change = checked(steps * Step(field, e.KeyModifiers)); }
        catch (OverflowException) { change = steps < 0 ? decimal.MinValue : decimal.MaxValue; }
        if (field.Minimum > field.Maximum) return;
        _candidate = Math.Clamp(AddBounded(_origin, change), field.Minimum, field.Maximum);
        if (_moved && _handle is { } handle)
        {
            ToolTip.SetTip(handle, _candidate.ToString("G", CultureInfo.CurrentCulture));
            ToolTip.SetIsOpen(handle, true);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_pointer != e.Pointer || AssociatedObject is not { } field) return;
        if (_moved) field.CommitValue(_candidate);
        else _input?.SelectAll();
        CancelScrub();
        e.Handled = true;
    }

    private void OnCaptureLost(object? sender, PointerCaptureLostEventArgs e) => CancelScrub();

    private void CancelScrub()
    {
        IPointer? pointer = _pointer;
        _pointer = null;
        if (_handle is { } handle) ToolTip.SetIsOpen(handle, false);
        pointer?.Capture(null);
    }

    private static decimal Step(StudioNumericField field, KeyModifiers modifiers)
    {
        try
        {
            return checked(field.Increment * (modifiers.HasFlag(KeyModifiers.Shift) ? 10m : modifiers.HasFlag(KeyModifiers.Alt) ? 0.1m : 1m));
        }
        catch (OverflowException) { return decimal.MaxValue; }
    }

    private static decimal AddBounded(decimal value, decimal change)
    {
        try { return checked(value + change); }
        catch (OverflowException) { return change < 0 ? decimal.MinValue : decimal.MaxValue; }
    }
}
