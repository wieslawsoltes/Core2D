// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Core2D.Controls;

public class ToggleItem : TemplatedControl
{
    public static readonly StyledProperty<object?> ToggleContentProperty = 
        AvaloniaProperty.Register<ToggleItem, object?>(nameof(ToggleContent));

    public static readonly StyledProperty<object?> PopupContentProperty = 
        AvaloniaProperty.Register<ToggleItem, object?>(nameof(PopupContent));

    public object? ToggleContent
    {
        get => GetValue(ToggleContentProperty);
        set => SetValue(ToggleContentProperty, value);
    }

    public object? PopupContent
    {
        get => GetValue(PopupContentProperty);
        set => SetValue(PopupContentProperty, value);
    }
}