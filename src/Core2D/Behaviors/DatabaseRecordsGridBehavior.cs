// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.DataGridFiltering;
using Avalonia.Controls.DataGridSearching;
using Avalonia.Controls.DataGridSorting;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using Core2D.ViewModels.Data;

namespace Core2D.Behaviors;

public sealed class DatabaseRecordsGridBehavior : Behavior<DataGrid>
{
    private static readonly object GlobalFilterKey = new();
    private readonly AvaloniaList<DataGridColumnDefinition> _columnDefinitions = new();
    private readonly Dictionary<ColumnViewModel, DataGridColumnDefinition> _columnLookup = new();
    private readonly FilteringModel _filteringModel = new();
    private readonly SortingModel _sortingModel = new();
    private readonly SearchModel _searchModel = new();
    private DataGridCollectionView? _recordsView;
    private DatabaseViewModel? _viewModel;

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is null)
        {
            return;
        }

        ConfigureGrid(AssociatedObject);
        AssociatedObject.DataContextChanged += OnDataContextChanged;
        UpdateViewModel(AssociatedObject.DataContext as DatabaseViewModel);
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject is { } grid)
        {
            grid.DataContextChanged -= OnDataContextChanged;
            grid.ItemsSource = null;
            grid.ColumnDefinitionsSource = null;
        }

        DetachViewModel();

        base.OnDetaching();
    }

    private void ConfigureGrid(DataGrid grid)
    {
        grid.ColumnDefinitionsSource = _columnDefinitions;
        grid.ColumnsSynchronizationMode = ColumnsSynchronizationMode.OneWayToGrid;
        grid.ColumnsSourceResetBehavior = ColumnsSourceResetBehavior.Reload;

        _filteringModel.OwnsViewFilter = true;
        _sortingModel.OwnsViewSorts = true;
        _sortingModel.MultiSort = true;
        _searchModel.HighlightMode = SearchHighlightMode.TextAndCell;
        _searchModel.HighlightCurrent = true;

        grid.FilteringModel = _filteringModel;
        grid.FilteringAdapterFactory = new DataGridAccessorFilteringAdapterFactory();
        grid.SortingModel = _sortingModel;
        grid.SearchModel = _searchModel;
        grid.SearchAdapterFactory = new DataGridAccessorSearchAdapterFactory();
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        UpdateViewModel(AssociatedObject?.DataContext as DatabaseViewModel);
    }

    private void UpdateViewModel(DatabaseViewModel? viewModel)
    {
        if (ReferenceEquals(_viewModel, viewModel))
        {
            return;
        }

        DetachViewModel();
        _viewModel = viewModel;

        if (_viewModel is null)
        {
            ClearRecordsView();
            ClearColumns();
            return;
        }

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        RebuildColumns();
        RebuildRecordsView();
        UpdateFilterAndSearch(_viewModel.RecordFilterText);
    }

    private void DetachViewModel()
    {
        if (_viewModel is null)
        {
            return;
        }

        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _viewModel = null;

        ClearColumnSubscriptions();
        ClearRecordsView();
        _filteringModel.Clear();
        _sortingModel.Clear();
        _searchModel.Clear();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_viewModel is null)
        {
            return;
        }

        if (e.PropertyName == nameof(DatabaseViewModel.Columns))
        {
            RebuildColumns();
            UpdateFilterAndSearch(_viewModel.RecordFilterText);
        }
        else if (e.PropertyName == nameof(DatabaseViewModel.Records))
        {
            RebuildRecordsView();
            UpdateFilterAndSearch(_viewModel.RecordFilterText);
        }
        else if (e.PropertyName == nameof(DatabaseViewModel.RecordFilterText))
        {
            UpdateFilterAndSearch(_viewModel.RecordFilterText);
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "DataGridCollectionView is required for ProDataGrid filtering and sorting.")]
    private void RebuildRecordsView()
    {
        if (_viewModel is null || AssociatedObject is null)
        {
            return;
        }

        _recordsView = new DataGridCollectionView(_viewModel.Records);
        AssociatedObject.ItemsSource = _recordsView;
    }

    private void ClearRecordsView()
    {
        _recordsView = null;

        if (AssociatedObject is { })
        {
            AssociatedObject.ItemsSource = null;
        }
    }

    private void RebuildColumns()
    {
        ClearColumnSubscriptions();
        _filteringModel.Clear();
        _sortingModel.Clear();

        if (_viewModel is null)
        {
            return;
        }

        for (var i = 0; i < _viewModel.Columns.Length; i++)
        {
            var column = _viewModel.Columns[i];
            var columnIndex = i;

            var accessor = new DataGridColumnValueAccessor<RecordViewModel, string?>(
                record => GetRecordValue(record, columnIndex),
                (record, value) => SetRecordValue(record, columnIndex, value));

            var options = new DataGridColumnDefinitionOptions
            {
                SortValueAccessor = accessor,
                SortValueComparer = StringComparer.OrdinalIgnoreCase,
                FilterValueAccessor = accessor,
                IsSearchable = true,
                SearchTextProvider = item => item is RecordViewModel record
                    ? GetRecordValue(record, columnIndex) ?? string.Empty
                    : string.Empty
            };

            var definition = new DataGridTextColumnDefinition
            {
                Header = column.Name,
                IsVisible = column.IsVisible,
                IsReadOnly = false,
                Width = DataGridLength.Auto,
                ColumnKey = column,
                ValueAccessor = accessor,
                ValueType = typeof(string),
                ShowFilterButton = true,
                Options = options
            };

            _columnDefinitions.Add(definition);
            _columnLookup[column] = definition;
            column.PropertyChanged += OnColumnPropertyChanged;
        }
    }

    private void ClearColumns()
    {
        ClearColumnSubscriptions();
        _columnDefinitions.Clear();
    }

    private void ClearColumnSubscriptions()
    {
        foreach (var column in _columnLookup.Keys)
        {
            column.PropertyChanged -= OnColumnPropertyChanged;
        }

        _columnLookup.Clear();
        _columnDefinitions.Clear();
    }

    private void OnColumnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is not ColumnViewModel column)
        {
            return;
        }

        if (!_columnLookup.TryGetValue(column, out var definition))
        {
            return;
        }

        if (e.PropertyName == nameof(ColumnViewModel.Name))
        {
            definition.Header = column.Name;
        }
        else if (e.PropertyName == nameof(ColumnViewModel.IsVisible))
        {
            definition.IsVisible = column.IsVisible;
        }
    }

    private void UpdateFilterAndSearch(string? query)
    {
        if (_viewModel is null)
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                _filteringModel.Remove(GlobalFilterKey);
                _searchModel.Clear();
                return;
            }

            var filterDescriptor = new FilteringDescriptor(
                GlobalFilterKey,
                FilteringOperator.Custom,
                string.Empty,
                query,
                Array.Empty<object>(),
                item => MatchesRecord(item as RecordViewModel, query),
                CultureInfo.CurrentCulture,
                StringComparison.OrdinalIgnoreCase);

            _filteringModel.SetOrUpdate(filterDescriptor);

            var searchDescriptor = new SearchDescriptor(
                query,
                SearchMatchMode.Contains,
                SearchTermCombineMode.Any,
                SearchScope.VisibleColumns,
                Array.Empty<object>(),
                StringComparison.OrdinalIgnoreCase,
                CultureInfo.CurrentCulture,
                wholeWord: false,
                normalizeWhitespace: true,
                ignoreDiacritics: true,
                allowEmpty: false);

            _searchModel.Apply(new[] { searchDescriptor });
        });
    }

    private static bool MatchesRecord(RecordViewModel? record, string query)
    {
        if (record is null)
        {
            return false;
        }

        foreach (var value in record.Values)
        {
            if (!string.IsNullOrWhiteSpace(value.Content) &&
                value.Content.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return true;
            }
        }

        return false;
    }

    private static string? GetRecordValue(RecordViewModel record, int index)
    {
        return index >= 0 && index < record.Values.Length
            ? record.Values[index].Content
            : null;
    }

    private static void SetRecordValue(RecordViewModel record, int index, string? value)
    {
        if (index >= 0 && index < record.Values.Length)
        {
            record.Values[index].Content = value;
        }
    }
}
