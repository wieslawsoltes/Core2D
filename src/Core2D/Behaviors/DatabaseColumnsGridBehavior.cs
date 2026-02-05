// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
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

public sealed class DatabaseColumnsGridBehavior : Behavior<DataGrid>
{
    private static readonly object GlobalFilterKey = new();
    private readonly AvaloniaList<DataGridColumnDefinition> _columnDefinitions = new();
    private readonly FilteringModel _filteringModel = new();
    private readonly SortingModel _sortingModel = new();
    private readonly SearchModel _searchModel = new();
    private DataGridCollectionView? _columnsView;
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

        EnsureColumnDefinitions();
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
            ClearColumnsView();
            return;
        }

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        RebuildColumnsView();
        UpdateFilterAndSearch(_viewModel.ColumnFilterText);
    }

    private void DetachViewModel()
    {
        if (_viewModel is null)
        {
            return;
        }

        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _viewModel = null;

        ClearColumnsView();
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
            RebuildColumnsView();
            UpdateFilterAndSearch(_viewModel.ColumnFilterText);
        }
        else if (e.PropertyName == nameof(DatabaseViewModel.ColumnFilterText))
        {
            UpdateFilterAndSearch(_viewModel.ColumnFilterText);
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "DataGridCollectionView is required for ProDataGrid filtering and sorting.")]
    private void RebuildColumnsView()
    {
        if (_viewModel is null || AssociatedObject is null)
        {
            return;
        }

        _columnsView = new DataGridCollectionView(_viewModel.Columns);
        AssociatedObject.ItemsSource = _columnsView;
    }

    private void ClearColumnsView()
    {
        _columnsView = null;

        if (AssociatedObject is { })
        {
            AssociatedObject.ItemsSource = null;
        }
    }

    private void EnsureColumnDefinitions()
    {
        if (_columnDefinitions.Count > 0)
        {
            return;
        }

        var nameAccessor = new DataGridColumnValueAccessor<ColumnViewModel, string?>(
            column => column.Name,
            (column, value) => column.Name = value ?? string.Empty);

        var nameOptions = new DataGridColumnDefinitionOptions
        {
            SortValueAccessor = nameAccessor,
            SortValueComparer = StringComparer.OrdinalIgnoreCase,
            FilterValueAccessor = nameAccessor,
            IsSearchable = true,
            SearchTextProvider = item => item is ColumnViewModel column
                ? column.Name ?? string.Empty
                : string.Empty
        };

        var nameDefinition = new DataGridTextColumnDefinition
        {
            Header = "Name",
            IsReadOnly = false,
            Width = DataGridLength.Auto,
            ValueAccessor = nameAccessor,
            ValueType = typeof(string),
            ShowFilterButton = true,
            Options = nameOptions
        };

        var visibleAccessor = new DataGridColumnValueAccessor<ColumnViewModel, bool>(
            column => column.IsVisible,
            (column, value) => column.IsVisible = value);

        var visibleOptions = new DataGridColumnDefinitionOptions
        {
            SortValueAccessor = visibleAccessor,
            FilterValueAccessor = visibleAccessor,
            IsSearchable = true,
            SearchTextProvider = item => item is ColumnViewModel column
                ? column.IsVisible.ToString(CultureInfo.InvariantCulture)
                : string.Empty
        };

        var visibleDefinition = new DataGridCheckBoxColumnDefinition
        {
            Header = "IsVisible",
            IsReadOnly = false,
            Width = DataGridLength.Auto,
            ValueAccessor = visibleAccessor,
            ValueType = typeof(bool),
            ShowFilterButton = true,
            Options = visibleOptions
        };

        var ownerAccessor = new DataGridColumnValueAccessor<ColumnViewModel, string?>(
            column => column.Owner?.Name,
            (_, _) => { });

        var ownerOptions = new DataGridColumnDefinitionOptions
        {
            SortValueAccessor = ownerAccessor,
            SortValueComparer = StringComparer.OrdinalIgnoreCase,
            FilterValueAccessor = ownerAccessor,
            IsSearchable = true,
            SearchTextProvider = item => item is ColumnViewModel column
                ? column.Owner?.Name ?? string.Empty
                : string.Empty
        };

        var ownerDefinition = new DataGridTextColumnDefinition
        {
            Header = "Owner",
            IsReadOnly = true,
            Width = DataGridLength.Auto,
            ValueAccessor = ownerAccessor,
            ValueType = typeof(string),
            ShowFilterButton = true,
            Options = ownerOptions
        };

        _columnDefinitions.Add(nameDefinition);
        _columnDefinitions.Add(visibleDefinition);
        _columnDefinitions.Add(ownerDefinition);
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
                item => MatchesColumn(item as ColumnViewModel, query),
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

    private static bool MatchesColumn(ColumnViewModel? column, string query)
    {
        if (column is null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(column.Name) &&
            column.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return true;
        }

        if (column.Owner is { } owner &&
            !string.IsNullOrWhiteSpace(owner.Name) &&
            owner.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return true;
        }

        return column.IsVisible.ToString(CultureInfo.InvariantCulture)
            .IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
