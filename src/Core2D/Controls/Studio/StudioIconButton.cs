// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Core2D.Controls.Studio;

/// <summary>A compact vector-icon command button with native button input and automation.</summary>
public class StudioIconButton : Button
{
    /// <summary>Defines the themeable vector icon.</summary>
    public static readonly StyledProperty<Geometry?> IconProperty =
        AvaloniaProperty.Register<StudioIconButton, Geometry?>(nameof(Icon));

    /// <summary>Gets or sets the vector icon displayed before the optional content.</summary>
    public Geometry? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}
