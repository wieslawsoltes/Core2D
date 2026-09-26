// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>Read-only result and selection status for an existing virtualized data grid.</summary>
public sealed class StudioTableStatus : TemplatedControl
{
    /// <summary>Defines the existing grid whose view and selection are observed.</summary>
    public static readonly DirectProperty<StudioTableStatus, DataGrid?> TargetProperty =
        AvaloniaProperty.RegisterDirect<StudioTableStatus, DataGrid?>(nameof(Target), x => x.Target, (x, value) => x.Target = value);
    /// <summary>Defines the source count; -1 means unknown rather than enumerating a lazy source.</summary>
    public static readonly DirectProperty<StudioTableStatus, int> TotalCountProperty =
        AvaloniaProperty.RegisterDirect<StudioTableStatus, int>(nameof(TotalCount), x => x.TotalCount, (x, value) => x.TotalCount = value);
    /// <summary>Defines the plural item label.</summary>
    public static readonly DirectProperty<StudioTableStatus, string> UnitProperty =
        AvaloniaProperty.RegisterDirect<StudioTableStatus, string>(nameof(Unit), x => x.Unit, (x, value) => x.Unit = value);
    /// <summary>Defines the observable result count; -1 means the view does not provide a constant-time count.</summary>
    public static readonly DirectProperty<StudioTableStatus, int> ResultCountProperty =
        AvaloniaProperty.RegisterDirect<StudioTableStatus, int>(nameof(ResultCount), x => x.ResultCount);
    /// <summary>Defines the formatted result summary.</summary>
    public static readonly DirectProperty<StudioTableStatus, string> SummaryProperty =
        AvaloniaProperty.RegisterDirect<StudioTableStatus, string>(nameof(Summary), x => x.Summary);
    /// <summary>Defines the selection summary.</summary>
    public static readonly DirectProperty<StudioTableStatus, string> SelectionSummaryProperty =
        AvaloniaProperty.RegisterDirect<StudioTableStatus, string>(nameof(SelectionSummary), x => x.SelectionSummary);
    private DataGrid? _target;
    private int _totalCount = -1, _resultCount = -1;
    private string _unit = "items", _summary = "", _selectionSummary = "";
    /// <summary>Initializes lifecycle-safe grid observation.</summary>
    public StudioTableStatus() => Interaction.GetBehaviors(this).Add(new StudioTableStatusBehavior());
    /// <summary>Gets or sets the grid without taking ownership of its models.</summary>
    public DataGrid? Target { get => _target; set => SetAndRaise(TargetProperty, ref _target, value); }
    /// <summary>Gets or sets the total source count.</summary>
    public int TotalCount { get => _totalCount; set => SetAndRaise(TotalCountProperty, ref _totalCount, value); }
    /// <summary>Gets or sets the plural item label.</summary>
    public string Unit { get => _unit; set => SetAndRaise(UnitProperty, ref _unit, value ?? "items"); }
    /// <summary>Gets the displayed view count without walking virtualized rows.</summary>
    public int ResultCount => _resultCount;
    /// <summary>Gets the result summary.</summary>
    public string Summary => _summary;
    /// <summary>Gets the selection count summary.</summary>
    public string SelectionSummary => _selectionSummary;
    internal void Update(int count, int selected)
    {
        SetAndRaise(ResultCountProperty, ref _resultCount, count);
        SetAndRaise(SummaryProperty, ref _summary, count < 0 ? $"{Unit} view" : TotalCount < 0
            ? $"{count} {Unit}" : $"{count} of {TotalCount} {Unit}");
        SetAndRaise(SelectionSummaryProperty, ref _selectionSummary, selected > 0 ? $"{selected} selected" : "No selection");
    }
}
