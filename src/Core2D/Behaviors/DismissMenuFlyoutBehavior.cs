// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Behaviors;

/// <summary>Dismisses the owning command menu after a leaf item is invoked.</summary>
public sealed class DismissMenuFlyoutBehavior : Behavior<Control>
{
    /// <summary>Defines the flyout to dismiss.</summary>
    public static readonly StyledProperty<FlyoutBase?> FlyoutProperty =
        AvaloniaProperty.Register<DismissMenuFlyoutBehavior, FlyoutBase?>(nameof(Flyout));

    /// <summary>Gets or sets the owning flyout.</summary>
    public FlyoutBase? Flyout
    {
        get => GetValue(FlyoutProperty);
        set => SetValue(FlyoutProperty, value);
    }

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject?.AddHandler(MenuItem.ClickEvent, OnMenuClick, RoutingStrategies.Bubble);
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        AssociatedObject?.RemoveHandler(MenuItem.ClickEvent, OnMenuClick);
        base.OnDetaching();
    }

    private void OnMenuClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source is MenuItem { ItemCount: 0 } && Flyout is { } flyout)
        {
            // Preserve the command's binding context until MenuItem has executed the command.
            Dispatcher.UIThread.Post(flyout.Hide, DispatcherPriority.Background);
        }
    }
}
