// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Core2D.Controls.Studio;

/// <summary>A native keyboard/automation-capable slider with a color ramp and optional transparency grid.</summary>
public class StudioColorSlider : Slider
{
    /// <summary>Defines the displayed color ramp.</summary>
    public static readonly StyledProperty<IBrush?> RampProperty = AvaloniaProperty.Register<StudioColorSlider, IBrush?>(nameof(Ramp));
    /// <summary>Defines whether a checkerboard is visible underneath the ramp.</summary>
    public static readonly StyledProperty<bool> ShowTransparencyProperty = AvaloniaProperty.Register<StudioColorSlider, bool>(nameof(ShowTransparency));
    /// <summary>Gets or sets the ramp.</summary>
    public IBrush? Ramp { get => GetValue(RampProperty); set => SetValue(RampProperty, value); }
    /// <summary>Gets or sets whether to show transparency.</summary>
    public bool ShowTransparency { get => GetValue(ShowTransparencyProperty); set => SetValue(ShowTransparencyProperty, value); }
}
