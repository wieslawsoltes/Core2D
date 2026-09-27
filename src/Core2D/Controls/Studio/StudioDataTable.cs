// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;
using Core2D.ViewModels.Data;

namespace Core2D.Controls.Studio;

/// <summary>A virtualized, searchable property/record inspector backed by ProDataGrid.</summary>
public class StudioDataTable : TemplatedControl
{
    /// <summary>Gets or sets the original data object or record.</summary>
    public static readonly DirectProperty<StudioDataTable, object?> SourceProperty =
        AvaloniaProperty.RegisterDirect<StudioDataTable, object?>(nameof(Source), x => x.Source, (x, value) => x.Source = value);
    private object? _source = null;
    /// <summary>Gets or sets the original data object or record.</summary>
    public object? Source { get => _source; set => SetAndRaise(SourceProperty, ref _source, value); }

    /// <summary>Gets or sets a case-insensitive name/value query.</summary>
    public static readonly DirectProperty<StudioDataTable, string?> QueryProperty =
        AvaloniaProperty.RegisterDirect<StudioDataTable, string?>(nameof(Query), x => x.Query, (x, value) => x.Query = value);
    private string? _query = null;
    /// <summary>Gets or sets a case-insensitive name/value query.</summary>
    public string? Query { get => _query; set => SetAndRaise(QueryProperty, ref _query, value); }

    /// <summary>Gets or sets original, name ascending, or name descending order.</summary>
    public static readonly DirectProperty<StudioDataTable, int> SortIndexProperty =
        AvaloniaProperty.RegisterDirect<StudioDataTable, int>(nameof(SortIndex), x => x.SortIndex, (x, value) => x.SortIndex = value);
    private int _sortIndex = 0;
    /// <summary>Gets or sets original, name ascending, or name descending order.</summary>
    public int SortIndex { get => _sortIndex; set => SetAndRaise(SortIndexProperty, ref _sortIndex, value); }

    /// <summary>Gets the current projected row count.</summary>
    public static readonly DirectProperty<StudioDataTable, int> ResultCountProperty =
        AvaloniaProperty.RegisterDirect<StudioDataTable, int>(nameof(ResultCount), x => x.ResultCount, (x, value) => x.ResultCount = value);
    private int _resultCount = 0;
    /// <summary>Gets the current projected row count.</summary>
    public int ResultCount { get => _resultCount; set => SetAndRaise(ResultCountProperty, ref _resultCount, value); }

    /// <summary>Gets the disposable row projection supplied by the behavior.</summary>
    public static readonly DirectProperty<StudioDataTable, DataFieldsViewModel?> EditorProperty =
        AvaloniaProperty.RegisterDirect<StudioDataTable, DataFieldsViewModel?>(nameof(Editor), x => x.Editor, (x, value) => x.Editor = value);
    private DataFieldsViewModel? _editor = null;
    /// <summary>Gets the disposable row projection supplied by the behavior.</summary>
    public DataFieldsViewModel? Editor { get => _editor; set => SetAndRaise(EditorProperty, ref _editor, value); }
    /// <summary>Defines an aggregate view of direct block-child properties rather than block-local properties.</summary>
    public static readonly DirectProperty<StudioDataTable, bool> IncludeChildPropertiesProperty =
        AvaloniaProperty.RegisterDirect<StudioDataTable, bool>(nameof(IncludeChildProperties), x => x.IncludeChildProperties, (x, value) => x.IncludeChildProperties = value);
    private bool _includeChildProperties;
    /// <summary>Gets or sets whether a block table includes each direct child's original properties.</summary>
    public bool IncludeChildProperties { get => _includeChildProperties; set => SetAndRaise(IncludeChildPropertiesProperty, ref _includeChildProperties, value); }
    /// <summary>Connects native grid projection and document-scoped editing services.</summary>
    public StudioDataTable() => Interaction.GetBehaviors(this).Add(new StudioDataTableBehavior());
}
