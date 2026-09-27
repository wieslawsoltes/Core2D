// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Handles the scoped primary+K shortcut and explicit Actions launcher for a workspace.</summary>
public sealed class StudioActionsHostBehavior : Behavior<Control>
{
    private StudioCommandPalette? Palette => AssociatedObject?.FindControl<StudioCommandPalette>("QuickActionsPalette");
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject?.AddHandler(InputElement.KeyDownEvent, OnKey, RoutingStrategies.Bubble);
        AssociatedObject?.AddHandler(Button.ClickEvent, OnClick);
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        AssociatedObject?.RemoveHandler(InputElement.KeyDownEvent, OnKey);
        AssociatedObject?.RemoveHandler(Button.ClickEvent, OnClick);
        if (Palette is { } palette) palette.IsOpen = false;
        base.OnDetaching();
    }
    private void OnKey(object? sender, KeyEventArgs e)
    {
        KeyModifiers primary = OperatingSystem.IsMacOS() ? KeyModifiers.Meta : KeyModifiers.Control;
        if (!e.Handled && e.Key == Key.K && e.KeyModifiers == primary && Palette is { CanOpen: true } palette)
        { palette.IsOpen = !palette.IsOpen; e.Handled = true; }
    }
    private void OnClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Button { Name: "QuickActionsButton" } && Palette is { CanOpen: true } palette)
        { palette.IsOpen = true; e.Handled = true; }
    }
}
