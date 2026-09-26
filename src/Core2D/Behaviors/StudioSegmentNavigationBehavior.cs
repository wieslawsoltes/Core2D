// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Implements directional, Home and End navigation without changing Tab semantics.</summary>
public sealed class StudioSegmentNavigationBehavior : Behavior<StudioSegmentedControl>
{
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject?.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        AssociatedObject?.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
        base.OnDetaching();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (AssociatedObject is not { IsEffectivelyEnabled: true } strip || strip.Items.Count == 0
            || e.KeyModifiers != KeyModifiers.None || e.Key is not (Key.Left or Key.Right or Key.Home or Key.End)) return;
        int direction = e.Key == Key.Left ? -1 : 1;
        if (strip.FlowDirection == FlowDirection.RightToLeft) direction = -direction;
        int index = e.Key switch
        {
            Key.Home => 0,
            Key.End => strip.Items.Count - 1,
            _ => strip.SelectedIndex < 0 ? (direction > 0 ? 0 : strip.Items.Count - 1) : strip.SelectedIndex + direction
        };
        if (e.Key == Key.Home) direction = 1;
        if (e.Key == Key.End) direction = -1;
        while (index >= 0 && index < strip.Items.Count)
        {
            Control? container = strip.ContainerFromIndex(index);
            if (container is not { IsEffectivelyEnabled: false } && container is not { IsVisible: false })
            {
                strip.SelectedIndex = index;
                container?.Focus(NavigationMethod.Directional);
                break;
            }
            index += direction;
        }
        e.Handled = true;
    }
}
