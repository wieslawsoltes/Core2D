// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>A compact numeric field preserving TextBox bindings and validation.</summary>
public class StudioNumberBox : TextBox
{
    /// <summary>Defines the field prefix and scrub handle.</summary>
    public static readonly StyledProperty<string?> PrefixProperty =
        AvaloniaProperty.Register<StudioNumberBox, string?>(nameof(Prefix));

    /// <summary>Defines the normal increment.</summary>
    public static readonly StyledProperty<double> StepProperty =
        AvaloniaProperty.Register<StudioNumberBox, double>(nameof(Step), 1d,
            validate: value => double.IsFinite(value) && value > 0);

    /// <summary>Initializes the numeric input behavior.</summary>
    public StudioNumberBox() => Interaction.GetBehaviors(this).Add(new StudioNumberInputBehavior());

    /// <summary>Gets or sets the prefix.</summary>
    public string? Prefix
    {
        get => GetValue(PrefixProperty);
        set => SetValue(PrefixProperty, value);
    }

    /// <summary>Gets or sets the increment. Shift multiplies by ten; Alt divides by ten.</summary>
    public double Step
    {
        get => GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }
}
