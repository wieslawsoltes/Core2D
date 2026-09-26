// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Core2D.Controls.Studio;

/// <summary>An icon or short-label toggle with native keyboard and toggle automation behavior.</summary>
public class StudioIconToggle : ToggleButton
{
    /// <summary>Defines the optional vector icon.</summary>
    public static readonly StyledProperty<Geometry?> IconProperty =
        AvaloniaProperty.Register<StudioIconToggle, Geometry?>(nameof(Icon));

    /// <summary>Gets or sets the icon; content remains available for short labels.</summary>
    public Geometry? Icon { get => GetValue(IconProperty); set => SetValue(IconProperty, value); }
}
