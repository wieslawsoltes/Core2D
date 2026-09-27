// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;

namespace Core2D.Controls.Studio;

/// <summary>A labelled inspector field that preserves the hosted editor's binding and validation.</summary>
public class StudioPropertyField : ContentControl
{
    /// <summary>Defines the field's visible label.</summary>
    public static readonly StyledProperty<string?> LabelProperty =
        AvaloniaProperty.Register<StudioPropertyField, string?>(nameof(Label));

    /// <summary>Gets or sets the label. Set the hosted input's automation name for accessible editing.</summary>
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
}
