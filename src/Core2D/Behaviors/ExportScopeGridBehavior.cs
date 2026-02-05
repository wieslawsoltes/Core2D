// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.DataGridFiltering;
using Avalonia.Controls.DataGridHierarchical;
using Avalonia.Controls.DataGridSearching;
using Avalonia.Controls.DataGridSorting;
using Avalonia.Xaml.Interactivity;
using Core2D.Helpers;
using Core2D.ViewModels.Wizard.Export.Scopes;
using Core2D.ViewModels.Wizard.Export.Steps;

namespace Core2D.Behaviors;

public sealed class ExportScopeGridBehavior : Behavior<DataGrid>
{
    private readonly AvaloniaList<DataGridColumnDefinition> _columnDefinitions = new();
    private readonly FilteringModel _filteringModel = new();
    private readonly SortingModel _sortingModel = new();
    private readonly SearchModel _searchModel = new();
    private readonly HierarchicalModel _hierarchicalModel;
    private DataGridCollectionView? _itemsView;
    private ScopeWizardStepViewModel? _viewModel;

    public ExportScopeGridBehavior()
    {
        _hierarchicalModel = CreateHierarchicalModel();
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject is null)
        {
            return;
        }

        ConfigureGrid(AssociatedObject);
        AssociatedObject.DataContextChanged += OnDataContextChanged;
        UpdateViewModel(AssociatedObject.DataContext as ScopeWizardStepViewModel);
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject is { } grid)
        {
            grid.DataContextChanged -= OnDataContextChanged;
            grid.ItemsSource = null;
            grid.ColumnDefinitionsSource = null;
            grid.HierarchicalModel = null;
        }

        DetachViewModel();

        base.OnDetaching();
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "HierarchicalModel is required for ProDataGrid hierarchy support.")]
    private static HierarchicalModel CreateHierarchicalModel()
    {
        var options = new HierarchicalOptions
        {
            ChildrenSelector = GetChildren,
            IsLeafSelector = IsLeaf,
            IsExpandedSelector = GetExpanded,
            IsExpandedSetter = SetExpanded
        };

        return new HierarchicalModel(options);
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

        ApplyHierarchy(grid);
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        UpdateViewModel(AssociatedObject?.DataContext as ScopeWizardStepViewModel);
    }

    private void UpdateViewModel(ScopeWizardStepViewModel? viewModel)
    {
        if (ReferenceEquals(_viewModel, viewModel))
        {
            return;
        }

        DetachViewModel();
        _viewModel = viewModel;

        if (_viewModel is null)
        {
            ClearItemsView();
            ClearColumns();
            return;
        }

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        RebuildColumns();
        RebuildItemsView();
    }

    private void DetachViewModel()
    {
        if (_viewModel is null)
        {
            return;
        }

        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _viewModel = null;

        ClearItemsView();
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

        if (e.PropertyName == nameof(ScopeWizardStepViewModel.Nodes))
        {
            RebuildItemsView();
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "DataGridCollectionView is required for ProDataGrid filtering and sorting.")]
    private void RebuildItemsView()
    {
        if (_viewModel is null || AssociatedObject is null)
        {
            return;
        }

        _hierarchicalModel.SetRoots(_viewModel.Nodes);
        _itemsView = new DataGridCollectionView(_hierarchicalModel.ObservableFlattened);
        AssociatedObject.ItemsSource = _itemsView;
        ApplyHierarchy(AssociatedObject);
    }

    private void ApplyHierarchy(DataGrid grid)
    {
        grid.HierarchicalModel = _hierarchicalModel;
        grid.HierarchicalRowsEnabled = true;
    }

    private void ClearItemsView()
    {
        _itemsView = null;

        if (AssociatedObject is { })
        {
            AssociatedObject.ItemsSource = null;
        }
    }

    private void RebuildColumns()
    {
        _columnDefinitions.Clear();
        _filteringModel.Clear();
        _sortingModel.Clear();
        _searchModel.Clear();

        var accessor = new DataGridColumnValueAccessor<object, string>(
            item => GridTextHelper.GetDisplayText(item),
            (item, value) => GridTextHelper.TrySetDisplayText(item, value));

        var options = new DataGridColumnDefinitionOptions
        {
            SortValueAccessor = accessor,
            SortValueComparer = StringComparer.OrdinalIgnoreCase,
            FilterValueAccessor = accessor,
            IsSearchable = true,
            SearchTextProvider = item => GridTextHelper.GetDisplayText(item)
        };

        var itemBinding = ColumnDefinitionBindingFactory.CreateBinding<HierarchicalNode, object?>(
            "Item",
            node => node.Item);

        var definition = new DataGridHierarchicalColumnDefinition
        {
            Header = string.Empty,
            IsReadOnly = true,
            Width = new DataGridLength(1, DataGridLengthUnitType.Star),
            ColumnKey = "Hierarchy",
            Binding = itemBinding,
            ValueAccessor = accessor,
            ValueType = typeof(string),
            ShowFilterButton = true,
            Options = options,
            Indent = 16,
            CellTemplateKey = "ExportScopeCellTemplate"
        };

        _columnDefinitions.Add(definition);
    }

    private void ClearColumns()
    {
        _columnDefinitions.Clear();
    }

    private static IEnumerable? GetChildren(object item)
    {
        return item is ExportScopeNodeViewModel scope ? scope.Children : null;
    }

    private static bool IsLeaf(object item)
    {
        return item is ExportScopeNodeViewModel scope && scope.Children.Count == 0;
    }

    private static bool? GetExpanded(object item)
    {
        return item is ExportScopeNodeViewModel scope ? scope.IsExpanded : null;
    }

    private static void SetExpanded(object item, bool value)
    {
        if (item is ExportScopeNodeViewModel scope)
        {
            scope.IsExpanded = value;
        }
    }
}
