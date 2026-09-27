// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Provides two-dimensional arrow navigation for the nine-point anchor control.</summary>
public sealed class StudioAnchorNavigationBehavior : Behavior<StudioAnchorPicker>
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
        if (AssociatedObject is not { IsEffectivelyEnabled: true } picker || e.KeyModifiers != KeyModifiers.None) return;
        int current = picker.SelectedIndex < 0 ? 0 : picker.SelectedIndex;
        int horizontal = picker.FlowDirection == FlowDirection.RightToLeft ? -1 : 1;
        int delta = e.Key switch { Key.Left => -horizontal, Key.Right => horizontal, Key.Up => -3, Key.Down => 3, _ => 0 };
        int next = e.Key switch { Key.Home => 0, Key.End => 8, _ => current + delta };
        if (delta == 0 && e.Key is not (Key.Home or Key.End)) return;
        if (e.Key is Key.Left or Key.Right && next / 3 != current / 3) next = current;
        if (next >= 0 && next < 9 && picker.ContainerFromIndex(next) is not { IsEffectivelyEnabled: false })
        {
            picker.SelectedIndex = next;
            picker.ContainerFromIndex(next)?.Focus(NavigationMethod.Directional);
        }
        e.Handled = true;
    }
}
