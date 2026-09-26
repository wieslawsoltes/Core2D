// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.DataGridFiltering;
using Avalonia.Controls.DataGridSearching;
using Avalonia.Controls.DataGridSorting;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;
using Core2D.ViewModels.Data;

namespace Core2D.Behaviors;

/// <summary>Connects a property table to explicit ProDataGrid accessors, projections and lifetimes.</summary>
public sealed class StudioDataTableBehavior : Behavior<StudioDataTable>
{
    private const string NameKey = "FieldName", ValueKey = "FieldValue";
    private DataGrid? _grid;
    private DataGridCollectionView? _view;
    private DataFieldsViewModel? _editor;
    private readonly List<DataFieldRowViewModel> _rows = new();
    private readonly FilteringModel _filtering = new() { OwnsViewFilter = true };
    private readonly SortingModel _sorting = new() { OwnsViewSorts = true, MultiSort = true };
    private readonly SearchModel _search = new();
    private int _generation;
    private bool _pending;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } table) return;
        table.TemplateApplied += OnTemplate;
        table.PropertyChanged += OnChanged;
        Connect(table.GetVisualDescendants().OfType<DataGrid>().FirstOrDefault(x => x.Name == "PART_Grid" && ReferenceEquals(x.TemplatedParent, table)));
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } table)
        {
            table.TemplateApplied -= OnTemplate;
            table.PropertyChanged -= OnChanged;
        }
        Disconnect();
        base.OnDetaching();
    }
    private void OnTemplate(object? sender, TemplateAppliedEventArgs e) => Connect(e.NameScope.Find<DataGrid>("PART_Grid"));
    private void Connect(DataGrid? grid)
    {
        Disconnect();
        _grid = grid;
        if (_grid is null) return;
        _grid.FilteringModel = _filtering;
        _grid.FilteringAdapterFactory = new DataGridAccessorFilteringAdapterFactory();
        _grid.SortingModel = _sorting;
        _grid.SearchModel = _search;
        _grid.SearchAdapterFactory = new DataGridAccessorSearchAdapterFactory();
        _grid.ColumnDefinitionsSource = new AvaloniaList<DataGridColumnDefinition>
        {
            Column(NameKey, "Name", "StudioDataNameCell", row => row.Name, 1),
            Column(ValueKey, "Value", "StudioDataValueCell", row => row.Value, 1.25),
            new DataGridTemplateColumnDefinition { Header = "", ColumnKey = "RemoveField", CellTemplateKey = "StudioDataRemoveCell", Width = new DataGridLength(34), IsReadOnly = true }
        };
        RebuildEditor();
    }
    private static DataGridColumnDefinition Column(string key, string header, string template,
        Func<DataFieldRowViewModel, string?> read, double width)
    {
        var accessor = new DataGridColumnValueAccessor<DataFieldRowViewModel, string?>(read, (_, _) => { });
        return new DataGridTemplateColumnDefinition
        {
            Header = header, ColumnKey = key, CellTemplateKey = template,
            Width = new DataGridLength(width, DataGridLengthUnitType.Star), IsReadOnly = true,
            ValueAccessor = accessor, ValueType = typeof(string), ShowFilterButton = true,
            Options = new DataGridColumnDefinitionOptions
            {
                SortValueAccessor = accessor, SortValueComparer = StringComparer.OrdinalIgnoreCase,
                FilterValueAccessor = accessor, IsSearchable = true,
                SearchTextProvider = item => item is DataFieldRowViewModel row ? read(row) ?? "" : ""
            }
        };
    }
    private void Disconnect()
    {
        if (_editor is not null) { _editor.PropertyChanged -= OnEditorChanged; _editor.Dispose(); }
        DropRows();
        _editor = null;
        if (AssociatedObject is { } table) { table.Editor = null; table.ResultCount = 0; }
        if (_grid is not null) _grid.ColumnDefinitionsSource = null;
        _grid = null;
        _filtering.Clear(); _sorting.Clear(); _search.Clear();
    }
    private void RebuildEditor()
    {
        if (_editor is not null) { _editor.PropertyChanged -= OnEditorChanged; _editor.Dispose(); }
        DropRows();
        _editor = null;
        if (_grid is null || AssociatedObject is not { } table) return;
        _editor = new DataFieldsViewModel(table.Source, StudioEditContext.GetHistory(table));
        table.Editor = _editor;
        _editor.PropertyChanged += OnEditorChanged;
        RebuildRows();
    }
    private void DropRows()
    {
        _generation++; _pending = false;
        foreach (DataFieldRowViewModel row in _rows) row.PropertyChanged -= OnRowChanged;
        _rows.Clear();
        if (_view is not null) _view.CollectionChanged -= OnViewChanged;
        if (_grid is not null) _grid.ItemsSource = null;
        if ((object?)_view is IDisposable disposable) disposable.Dispose();
        _view = null;
    }
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "ProDataGrid uses explicit typed accessors and compiled cell templates, never property-name reflection.")]
    private void RebuildRows()
    {
        DropRows();
        if (_grid is null || _editor is null) return;
        foreach (DataFieldRowViewModel row in _editor.Rows) { _rows.Add(row); row.PropertyChanged += OnRowChanged; }
        _view = new DataGridCollectionView(_editor.Rows);
        _view.CollectionChanged += OnViewChanged;
        _grid.ItemsSource = _view;
        ApplyQuery();
    }
    private void ApplyQuery()
    {
        if (_grid is null || AssociatedObject is not { } table) return;
        string query = table.Query?.Trim() ?? "";
        if (query.Length == 0) { _filtering.Remove("Query"); _search.Clear(); }
        else
        {
            _filtering.SetOrUpdate(new FilteringDescriptor("Query", FilteringOperator.Custom, string.Empty, query,
                Array.Empty<object>(), item => item is DataFieldRowViewModel row &&
                (row.Name.Contains(query, StringComparison.OrdinalIgnoreCase) || (row.Value?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)),
                CultureInfo.CurrentCulture, StringComparison.OrdinalIgnoreCase));
            _search.Apply(new[] { new SearchDescriptor(query, SearchMatchMode.Contains, SearchTermCombineMode.Any,
                SearchScope.VisibleColumns, Array.Empty<object>(), StringComparison.OrdinalIgnoreCase,
                CultureInfo.CurrentCulture, wholeWord: false, normalizeWhitespace: true, ignoreDiacritics: false, allowEmpty: false) });
        }
        if (table.SortIndex == 0) _sorting.Clear();
        else _sorting.Apply(new[] { new SortingDescriptor(NameKey, table.SortIndex == 1 ? ListSortDirection.Ascending : ListSortDirection.Descending) });
        table.ResultCount = _view?.Count ?? 0;
    }
    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == StudioDataTable.SourceProperty || e.Property == StudioEditContext.HistoryProperty) RebuildEditor();
        else if (e.Property == StudioDataTable.QueryProperty || e.Property == StudioDataTable.SortIndexProperty) ApplyQuery();
    }
    private void OnEditorChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(DataFieldsViewModel.Rows)) RebuildRows();
    }
    private void OnRowChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_pending || e.PropertyName is not (nameof(DataFieldRowViewModel.Name) or nameof(DataFieldRowViewModel.Value))) return;
        _pending = true;
        int version = _generation;
        // Do not recycle a cell while its user-commit callback is still on the stack.
        Dispatcher.UIThread.Post(() =>
        {
            if (version != _generation) return;
            _pending = false;
            _view?.Refresh();
            if (AssociatedObject is { } table) table.ResultCount = _view?.Count ?? 0;
        });
    }
    private void OnViewChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (AssociatedObject is { } table) table.ResultCount = _view?.Count ?? 0;
    }
}
