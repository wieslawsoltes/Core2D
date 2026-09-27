// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>Native find-next navigation over a grid's existing searchable columns and expanded hierarchy.</summary>
public sealed class StudioFindBar : TemplatedControl
{
    /// <summary>Defines the existing grid; no alternate row projection is created.</summary>
    public static readonly DirectProperty<StudioFindBar, DataGrid?> TargetProperty =
        AvaloniaProperty.RegisterDirect<StudioFindBar, DataGrid?>(nameof(Target), x => x.Target, (x, value) => x.Target = value);
    /// <summary>Defines the literal search query.</summary>
    public static readonly DirectProperty<StudioFindBar, string?> QueryProperty =
        AvaloniaProperty.RegisterDirect<StudioFindBar, string?>(nameof(Query), x => x.Query, (x, value) => x.Query = value);
    /// <summary>Defines contextual search guidance.</summary>
    public static readonly DirectProperty<StudioFindBar, string> WatermarkProperty =
        AvaloniaProperty.RegisterDirect<StudioFindBar, string>(nameof(Watermark), x => x.Watermark, (x, value) => x.Watermark = value);
    /// <summary>Defines the current match position.</summary>
    public static readonly DirectProperty<StudioFindBar, string> MatchSummaryProperty =
        AvaloniaProperty.RegisterDirect<StudioFindBar, string>(nameof(MatchSummary), x => x.MatchSummary);
    /// <summary>Defines whether the current search has results.</summary>
    public static readonly DirectProperty<StudioFindBar, bool> HasMatchesProperty =
        AvaloniaProperty.RegisterDirect<StudioFindBar, bool>(nameof(HasMatches), x => x.HasMatches);
    /// <summary>Defines whether hierarchy expansion actions are supported.</summary>
    public static readonly DirectProperty<StudioFindBar, bool> HasHierarchyProperty =
        AvaloniaProperty.RegisterDirect<StudioFindBar, bool>(nameof(HasHierarchy), x => x.HasHierarchy);
    private DataGrid? _target;
    private string? _query;
    private string _watermark = "Find in expanded objects…", _summary = "";
    private bool _hasMatches, _hasHierarchy;
    /// <summary>Connects native search navigation and keyboard handling.</summary>
    public StudioFindBar() => Interaction.GetBehaviors(this).Add(new StudioFindBarBehavior());
    /// <summary>Gets or sets the target grid.</summary>
    public DataGrid? Target { get => _target; set => SetAndRaise(TargetProperty, ref _target, value); }
    /// <summary>Gets or sets the search text; the underlying source and filter are left unchanged.</summary>
    public string? Query { get => _query; set => SetAndRaise(QueryProperty, ref _query, value); }
    /// <summary>Gets or sets the contextual prompt.</summary>
    public string Watermark { get => _watermark; set => SetAndRaise(WatermarkProperty, ref _watermark, value); }
    /// <summary>Gets the match position summary.</summary>
    public string MatchSummary => _summary;
    /// <summary>Gets whether match navigation can run.</summary>
    public bool HasMatches => _hasMatches;
    /// <summary>Gets whether the target has a hierarchy model.</summary>
    public bool HasHierarchy => _hasHierarchy;
    internal void Update(int count, int index, bool hierarchy)
    {
        SetAndRaise(HasMatchesProperty, ref _hasMatches, count > 0);
        SetAndRaise(HasHierarchyProperty, ref _hasHierarchy, hierarchy);
        SetAndRaise(MatchSummaryProperty, ref _summary, string.IsNullOrWhiteSpace(Query) ? "" : count == 0 ? "No matches" : $"{index + 1} of {count}");
    }
}
