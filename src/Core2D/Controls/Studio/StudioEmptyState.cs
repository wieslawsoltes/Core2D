// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;

namespace Core2D.Controls.Studio;

/// <summary>A theme-aware empty-state surface with optional actions.</summary>
public class StudioEmptyState : ContentControl
{
    /// <summary>Gets or sets the empty-state title.</summary>
    public static readonly DirectProperty<StudioEmptyState, string?> TitleProperty =
        AvaloniaProperty.RegisterDirect<StudioEmptyState, string?>(nameof(Title), x => x.Title, (x, value) => x.Title = value);
    private string? _title = null;
    /// <summary>Gets or sets the empty-state title.</summary>
    public string? Title { get => _title; set => SetAndRaise(TitleProperty, ref _title, value); }

    /// <summary>Gets or sets the explanatory text.</summary>
    public static readonly DirectProperty<StudioEmptyState, string?> DescriptionProperty =
        AvaloniaProperty.RegisterDirect<StudioEmptyState, string?>(nameof(Description), x => x.Description, (x, value) => x.Description = value);
    private string? _description = null;
    /// <summary>Gets or sets the explanatory text.</summary>
    public string? Description { get => _description; set => SetAndRaise(DescriptionProperty, ref _description, value); }

}
