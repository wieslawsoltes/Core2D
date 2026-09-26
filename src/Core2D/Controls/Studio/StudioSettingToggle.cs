// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls.Primitives;

namespace Core2D.Controls.Studio;

/// <summary>A full-row native toggle with a concise label, supporting description and mixed-state track.</summary>
public sealed class StudioSettingToggle : ToggleButton
{
    /// <summary>Defines optional contextual help beneath the inherited Content label.</summary>
    public static readonly DirectProperty<StudioSettingToggle, string?> DescriptionProperty =
        AvaloniaProperty.RegisterDirect<StudioSettingToggle, string?>(nameof(Description), x => x.Description, (x, value) => x.Description = value);
    private string? _description;
    /// <summary>Gets or sets contextual help. Empty descriptions reserve no space.</summary>
    public string? Description { get => _description; set => SetAndRaise(DescriptionProperty, ref _description, value); }
}
