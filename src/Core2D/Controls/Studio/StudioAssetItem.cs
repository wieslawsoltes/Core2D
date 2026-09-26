// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Core2D.Controls.Studio;

/// <summary>A compact asset row with optional preview content and native naming editor.</summary>
public class StudioAssetItem : ContentControl
{
    /// <summary>Gets or sets the asset name.</summary>
    public static readonly DirectProperty<StudioAssetItem, string?> TitleProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetItem, string?>(nameof(Title), x => x.Title, (x, value) => x.Title = value);
    private string? _title = null;
    /// <summary>Gets or sets the asset name.</summary>
    public string? Title { get => _title; set => SetAndRaise(TitleProperty, ref _title, value); }

    /// <summary>Gets or sets the asset metadata.</summary>
    public static readonly DirectProperty<StudioAssetItem, string?> DetailProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetItem, string?>(nameof(Detail), x => x.Detail, (x, value) => x.Detail = value);
    private string? _detail = null;
    /// <summary>Gets or sets the asset metadata.</summary>
    public string? Detail { get => _detail; set => SetAndRaise(DetailProperty, ref _detail, value); }

    /// <summary>Gets or sets the fallback vector icon.</summary>
    public static readonly DirectProperty<StudioAssetItem, Geometry?> IconProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetItem, Geometry?>(nameof(Icon), x => x.Icon, (x, value) => x.Icon = value);
    private Geometry? _icon = null;
    /// <summary>Gets or sets the fallback vector icon.</summary>
    public Geometry? Icon { get => _icon; set => SetAndRaise(IconProperty, ref _icon, value); }

    /// <summary>Gets or sets whether preview details are suppressed.</summary>
    public static readonly DirectProperty<StudioAssetItem, bool> IsCompactProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetItem, bool>(nameof(IsCompact), x => x.IsCompact, (x, value) => x.IsCompact = value);
    private bool _isCompact = false;
    /// <summary>Gets or sets whether preview details are suppressed.</summary>
    public bool IsCompact { get => _isCompact; set => SetAndRaise(IsCompactProperty, ref _isCompact, value); }

}
