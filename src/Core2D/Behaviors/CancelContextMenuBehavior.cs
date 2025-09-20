// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Core2D.Behaviors;

public sealed class EnableContextMenuBehavior : Behavior<Control>
{
    public static readonly StyledProperty<bool> IsEnabledProperty =
        AvaloniaProperty.Register<EnableContextMenuBehavior, bool>(nameof(IsEnabled), true);

    public static readonly StyledProperty<ContextMenu> ContextMenuProperty =
        AvaloniaProperty.Register<EnableContextMenuBehavior, ContextMenu>(nameof(ContextMenu));

    public bool IsEnabled
    {
        get => GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    public ContextMenu ContextMenu
    {
        get => GetValue(ContextMenuProperty);
        set => SetValue(ContextMenuProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        if ((ContextMenu ?? AssociatedObject?.ContextMenu) is { } contextMenu)
        {
            contextMenu.Opening += ContextMenu_ContextMenuOpening;
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();

        if ((ContextMenu ?? AssociatedObject?.ContextMenu) is { } contextMenu)
        {
            contextMenu.Opening -= ContextMenu_ContextMenuOpening;
        }
    }

    private void ContextMenu_ContextMenuOpening(object? sender, CancelEventArgs e)
    {
        e.Cancel = !IsEnabled;
    }
}
