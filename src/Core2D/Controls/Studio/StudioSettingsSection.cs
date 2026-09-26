// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;

namespace Core2D.Controls.Studio;

/// <summary>A studio section with explicit search metadata independent of its model values.</summary>
public class StudioSettingsSection : StudioSection
{
    /// <summary>Defines additional discoverability terms for this settings group.</summary>
    public static readonly DirectProperty<StudioSettingsSection, string?> KeywordsProperty =
        AvaloniaProperty.RegisterDirect<StudioSettingsSection, string?>(nameof(Keywords), x => x.Keywords, (x, value) => x.Keywords = value);
    /// <summary>Defines whether the containing settings search matches this section.</summary>
    public static readonly DirectProperty<StudioSettingsSection, bool> MatchesQueryProperty =
        AvaloniaProperty.RegisterDirect<StudioSettingsSection, bool>(nameof(MatchesQuery), x => x.MatchesQuery);
    private string? _keywords;
    private bool _matchesQuery = true;
    /// <summary>Gets or sets search terms; these do not change the edited model.</summary>
    public string? Keywords { get => _keywords; set => SetAndRaise(KeywordsProperty, ref _keywords, value); }
    /// <summary>Gets whether the section matches the search; a style controls its presentation.</summary>
    public bool MatchesQuery => _matchesQuery;
    internal void SetMatch(bool match) => SetAndRaise(MatchesQueryProperty, ref _matchesQuery, match);
}
