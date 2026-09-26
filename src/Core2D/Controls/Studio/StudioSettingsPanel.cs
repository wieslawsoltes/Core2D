// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>A bounded, searchable host for native settings sections, preserving expansion state.</summary>
public class StudioSettingsPanel : ItemsControl
{
    /// <summary>Gets or sets a case-insensitive group/keyword search.</summary>
    public static readonly DirectProperty<StudioSettingsPanel, string?> QueryProperty =
        AvaloniaProperty.RegisterDirect<StudioSettingsPanel, string?>(nameof(Query), x => x.Query, (x, value) => x.Query = value);
    private string? _query = null;
    /// <summary>Gets or sets a case-insensitive group/keyword search.</summary>
    public string? Query { get => _query; set => SetAndRaise(QueryProperty, ref _query, value); }

    /// <summary>Gets the number of matching settings groups.</summary>
    public static readonly DirectProperty<StudioSettingsPanel, int> MatchCountProperty =
        AvaloniaProperty.RegisterDirect<StudioSettingsPanel, int>(nameof(MatchCount), x => x.MatchCount, (x, value) => x.MatchCount = value);
    private int _matchCount = 0;
    /// <summary>Gets the number of matching settings groups.</summary>
    public int MatchCount { get => _matchCount; set => SetAndRaise(MatchCountProperty, ref _matchCount, value); }
    /// <summary>Initializes explicit section search and lifecycle handling.</summary>
    public StudioSettingsPanel() => Interaction.GetBehaviors(this).Add(new StudioSettingsSearchBehavior());
}
