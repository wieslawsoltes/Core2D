// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Scopes row adapters and inline editing to realized hierarchy rows.</summary>
public sealed class StudioLayerRowBehavior : Behavior<StudioLayerRow>
{
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } row) return;
        row.PropertyChanged += OnChanged;
        row.AttachedToVisualTree += OnVisualAttached;
        row.DoubleTapped += OnDoubleTapped;
        row.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Bubble, true);
        row.AddHandler(InputElement.LostFocusEvent, OnLostFocus, RoutingStrategies.Bubble);
        row.Rebind(row.GetVisualRoot() is not null);
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } row)
        {
            row.PropertyChanged -= OnChanged;
            row.AttachedToVisualTree -= OnVisualAttached;
            row.DoubleTapped -= OnDoubleTapped;
            row.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
            row.RemoveHandler(InputElement.LostFocusEvent, OnLostFocus);
            row.Rebind(false);
        }
        base.OnDetaching();
    }
    private void OnVisualAttached(object? sender, VisualTreeAttachmentEventArgs e) => AssociatedObject?.Rebind(true);
    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (AssociatedObject is not { } row) return;
        if (e.Property == StyledElement.DataContextProperty || e.Property == StudioEditContext.HistoryProperty) row.Rebind(row.GetVisualRoot() is not null);
        else if (e.Property == StudioLayerRow.IsRenamingProperty && row.IsRenaming)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (!ReferenceEquals(AssociatedObject, row) || !row.IsRenaming) return;
                TextBox? input = row.GetVisualDescendants().OfType<TextBox>().FirstOrDefault();
                input?.Focus(); input?.SelectAll();
            }, DispatcherPriority.Input);
        }
    }
    private void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (AssociatedObject is not { } row || row.IsRenaming) return;
        for (Visual? visual = e.Source as Visual; visual is not null && visual != row; visual = visual.GetVisualParent())
            if (visual is Button or TextBox) return;
        row.IsRenaming = true;
        e.Handled = true;
    }
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (AssociatedObject is not { } row) return;
        if (e.Key == Key.F2) { row.IsRenaming = true; e.Handled = true; }
        else if (row.IsRenaming && e.Key is Key.Enter or Key.Escape)
        {
            StudioNameField? field = row.GetVisualDescendants().OfType<StudioNameField>().FirstOrDefault();
            // The field owns native composition and validation; keep invalid names editable.
            if (e.Key == Key.Escape || field?.Error is null) row.IsRenaming = false;
            e.Handled = true;
        }
    }
    private void OnLostFocus(object? sender, RoutedEventArgs e)
    {
        StudioLayerRow? row = AssociatedObject;
        Dispatcher.UIThread.Post(() =>
        {
            if (row is not null && ReferenceEquals(AssociatedObject, row) && !row.IsKeyboardFocusWithin) row.IsRenaming = false;
        }, DispatcherPriority.Input);
    }
}
