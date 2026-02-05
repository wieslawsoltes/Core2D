// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
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
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Behaviors;

public sealed class ObjectBrowserGridBehavior : Behavior<DataGrid>
{
    private readonly AvaloniaList<DataGridColumnDefinition> _columnDefinitions = new();
    private readonly FilteringModel _filteringModel = new();
    private readonly SortingModel _sortingModel = new();
    private readonly SearchModel _searchModel = new();
    private readonly HierarchicalModel _hierarchicalModel;
    private ObjectBrowserState? _state;
    private DataGridCollectionView? _itemsView;
    private ProjectEditorViewModel? _viewModel;
    private ProjectContainerViewModel? _project;
    private IDisposable? _hierarchySubscription;

    public ObjectBrowserGridBehavior()
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
        UpdateViewModel(AssociatedObject.DataContext as ProjectEditorViewModel);
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
    private HierarchicalModel CreateHierarchicalModel()
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
        UpdateViewModel(AssociatedObject?.DataContext as ProjectEditorViewModel);
    }

    private void UpdateViewModel(ProjectEditorViewModel? viewModel)
    {
        if (ReferenceEquals(_viewModel, viewModel))
        {
            return;
        }

        DetachViewModel();
        _viewModel = viewModel;
        _state = _viewModel?.ObjectBrowserState;

        if (_viewModel is null)
        {
            ClearItemsView();
            ClearColumns();
            return;
        }

        _viewModel.PropertyChanged += OnViewModelPropertyChanged;
        UpdateProject(_viewModel.Project);
        RebuildColumns();
        EnsureItemsView();
    }

    private void DetachViewModel()
    {
        if (_viewModel is null)
        {
            return;
        }

        _viewModel.PropertyChanged -= OnViewModelPropertyChanged;
        _viewModel = null;
        _state = null;

        DetachProject();
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

        if (e.PropertyName == nameof(ProjectEditorViewModel.Project))
        {
            UpdateProject(_viewModel.Project);
        }
    }

    private void UpdateProject(ProjectContainerViewModel? project)
    {
        if (ReferenceEquals(_project, project))
        {
            return;
        }

        DetachProject();
        _project = project;

        if (_project is { })
        {
            _project.PropertyChanged += OnProjectPropertyChanged;
            _hierarchySubscription = _project.Subscribe(new HierarchyObserver(this));
        }

        _state?.UpdateProject(_project);
        RefreshHierarchy();
    }

    private void DetachProject()
    {
        if (_project is { })
        {
            _project.PropertyChanged -= OnProjectPropertyChanged;
            _project = null;
        }

        DetachHierarchySubscription();
    }

    private void OnProjectPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_project is null)
        {
            return;
        }

        if (e.PropertyName == nameof(ProjectContainerViewModel.Name))
        {
            _state?.UpdateProject(_project);
            RefreshHierarchy();
        }
        else if (e.PropertyName is nameof(ProjectContainerViewModel.StyleLibraries)
                 or nameof(ProjectContainerViewModel.GroupLibraries)
                 or nameof(ProjectContainerViewModel.Databases)
                 or nameof(ProjectContainerViewModel.Templates)
                 or nameof(ProjectContainerViewModel.Scripts)
                 or nameof(ProjectContainerViewModel.Documents))
        {
            RefreshHierarchy();
        }
    }

    private void DetachHierarchySubscription()
    {
        if (_hierarchySubscription is null)
        {
            return;
        }

        _hierarchySubscription.Dispose();
        _hierarchySubscription = null;
    }

    private void OnHierarchyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_project is null)
        {
            return;
        }

        if (e.PropertyName is nameof(ProjectContainerViewModel.StyleLibraries)
            or nameof(ProjectContainerViewModel.GroupLibraries)
            or nameof(ProjectContainerViewModel.Databases)
            or nameof(ProjectContainerViewModel.Templates)
            or nameof(ProjectContainerViewModel.Scripts)
            or nameof(ProjectContainerViewModel.Documents)
            or nameof(DocumentContainerViewModel.Pages)
            or nameof(PageContainerViewModel.Layers)
            or nameof(TemplateContainerViewModel.Layers)
            or nameof(LayerContainerViewModel.Shapes)
            or nameof(BlockShapeViewModel.Shapes)
            or nameof(PathShapeViewModel.Figures)
            or nameof(PathFigureViewModel.Segments)
            or nameof(LibraryViewModel.Items))
        {
            RefreshHierarchy(sender);
        }
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "DataGridCollectionView is required for ProDataGrid filtering and sorting.")]
    private void EnsureItemsView()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        RefreshHierarchy();
        _itemsView ??= new DataGridCollectionView(_hierarchicalModel.ObservableFlattened);
        AssociatedObject.ItemsSource = _itemsView;
        ApplyHierarchy(AssociatedObject);
    }

    private void ClearItemsView()
    {
        _itemsView = null;

        if (AssociatedObject is { })
        {
            AssociatedObject.ItemsSource = null;
        }
    }

    private void RefreshHierarchy()
    {
        if (_state is null)
        {
            _hierarchicalModel.SetRoots(Array.Empty<object>());
            return;
        }

        _hierarchicalModel.SetRoots(_state.RootItems);
    }

    private void ApplyHierarchy(DataGrid grid)
    {
        grid.HierarchicalModel = _hierarchicalModel;
        grid.HierarchicalRowsEnabled = true;
    }

    private void RefreshHierarchy(object? item)
    {
        if (item is null)
        {
            return;
        }

        var node = _hierarchicalModel.FindNode(item);
        if (node is not null)
        {
            _hierarchicalModel.Refresh(node);
            return;
        }

        if (_hierarchicalModel.Root is { } root)
        {
            _hierarchicalModel.Refresh(root);
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
            CellTemplateKey = "ObjectBrowserCellTemplate"
        };

        _columnDefinitions.Add(definition);
    }

    private void ClearColumns()
    {
        _columnDefinitions.Clear();
    }

    private IEnumerable? GetChildren(object item)
    {
        return item switch
        {
            ObjectBrowserNode node => node.ChildrenSelector(_project),
            DocumentContainerViewModel document => document.Pages,
            PageContainerViewModel page => page.Layers,
            TemplateContainerViewModel template => template.Layers,
            LayerContainerViewModel layer => layer.Shapes,
            BlockShapeViewModel block => block.Shapes,
            PathShapeViewModel path => path.Figures,
            PathFigureViewModel figure => figure.Segments,
            LibraryViewModel library => library.Items,
            _ => null
        };
    }

    private static bool IsLeaf(object item)
    {
        return item switch
        {
            ObjectBrowserNode => false,
            DocumentContainerViewModel => false,
            PageContainerViewModel => false,
            TemplateContainerViewModel => false,
            LayerContainerViewModel => false,
            BlockShapeViewModel => false,
            PathShapeViewModel => false,
            PathFigureViewModel => false,
            LibraryViewModel => false,
            _ => true
        };
    }

    private static bool? GetExpanded(object item)
    {
        return item switch
        {
            ObjectBrowserNode node => node.IsExpanded,
            BaseContainerViewModel container => container.IsExpanded,
            _ => null
        };
    }

    private static void SetExpanded(object item, bool value)
    {
        switch (item)
        {
            case ObjectBrowserNode node:
                node.IsExpanded = value;
                break;
            case BaseContainerViewModel container:
                container.IsExpanded = value;
                break;
        }
    }

    private sealed class HierarchyObserver : IObserver<(object? sender, PropertyChangedEventArgs e)>
    {
        private readonly ObjectBrowserGridBehavior _owner;

        public HierarchyObserver(ObjectBrowserGridBehavior owner)
        {
            _owner = owner;
        }

        public void OnNext((object? sender, PropertyChangedEventArgs e) value)
        {
            _owner.OnHierarchyChanged(value.sender, value.e);
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }
    }
}
