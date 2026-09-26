// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;

namespace Core2D.Controls.Studio;

/// <summary>An inline inspector row with a consistent label column.</summary>
public class StudioPropertyRow : ContentControl
{
    /// <summary>Defines the row label.</summary>
    public static readonly StyledProperty<string?> LabelProperty = AvaloniaProperty.Register<StudioPropertyRow, string?>(nameof(Label));
    /// <summary>Defines the label width.</summary>
    public static readonly StyledProperty<double> LabelWidthProperty = AvaloniaProperty.Register<StudioPropertyRow, double>(nameof(LabelWidth), 76);
    /// <summary>Gets or sets the label.</summary>
    public string? Label { get => GetValue(LabelProperty); set => SetValue(LabelProperty, value); }
    /// <summary>Gets or sets the label width.</summary>
    public double LabelWidth { get => GetValue(LabelWidthProperty); set => SetValue(LabelWidthProperty, value); }
}
