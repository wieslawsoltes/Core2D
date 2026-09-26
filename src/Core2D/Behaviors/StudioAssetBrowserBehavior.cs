// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;
using Core2D.ViewModels;
using Core2D.ViewModels.Renderer;

namespace Core2D.Behaviors;

/// <summary>Owns the asset grid projection, explicit accessors and subscriptions for one browser.</summary>
public sealed class StudioAssetBrowserBehavior : Behavior<StudioAssetBrowser>
{
    private const string ColumnKey = "AssetName";
    private DataGrid? _grid;
    private DataGridCollectionView? _view;
    private INotifyCollectionChanged? _source;
    private readonly List<INotifyPropertyChanged> _observed = new();
    private readonly FilteringModel _filtering = new() { OwnsViewFilter = true };
    private readonly SortingModel _sorting = new() { OwnsViewSorts = true };
    private readonly SearchModel _search = new();
    private bool _sync;
    private int _generation;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } browser) return;
        browser.TemplateApplied += OnTemplate;
        browser.PropertyChanged += OnChanged;
        Connect(browser.GetVisualDescendants().OfType<DataGrid>().FirstOrDefault(x => x.Name == "PART_Grid" && ReferenceEquals(x.TemplatedParent, browser)));
    }

    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } browser)
        {
            browser.TemplateApplied -= OnTemplate;
            browser.PropertyChanged -= OnChanged;
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
        _grid.SelectionChanged += OnSelectionChanged;
        _grid.DoubleTapped += OnOpen;
        _grid.AddHandler(InputElement.KeyDownEvent, OnKey, RoutingStrategies.Bubble);
        _grid.FilteringModel = _filtering;
        _grid.FilteringAdapterFactory = new DataGridAccessorFilteringAdapterFactory();
        _grid.SortingModel = _sorting;
        _grid.SearchModel = _search;
        _grid.SearchAdapterFactory = new DataGridAccessorSearchAdapterFactory();
        ConfigureColumn();
        Rebuild();
    }

    private void Disconnect()
    {
        _generation++;
        DetachSource();
        if (_grid is { } grid)
        {
            grid.SelectionChanged -= OnSelectionChanged;
            grid.DoubleTapped -= OnOpen;
            grid.RemoveHandler(InputElement.KeyDownEvent, OnKey);
            grid.ItemsSource = null;
            grid.ColumnDefinitionsSource = null;
        }
        if (_view is not null) _view.CollectionChanged -= OnViewChanged;
        if ((object?)_view is IDisposable disposable) disposable.Dispose();
        _view = null;
        _grid = null;
        _filtering.Clear(); _sorting.Clear(); _search.Clear();
    }

    private void DetachSource()
    {
        if (_source is not null) _source.CollectionChanged -= OnCollectionChanged;
        _source = null;
        foreach (INotifyPropertyChanged item in _observed) item.PropertyChanged -= OnItemChanged;
        _observed.Clear();
    }

    private void ConfigureColumn()
    {
        if (_grid is null || AssociatedObject is not { } browser) return;
        var accessor = new DataGridColumnValueAccessor<object, string>(Name, (_, _) => { });
        DataGridColumnDefinition column = browser.ItemTemplate is { } template
            ? new DataGridTemplateColumnDefinition { CellTemplate = template }
            : new DataGridTextColumnDefinition();
        column.ColumnKey = ColumnKey;
        column.Header = "Name";
        column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        column.IsReadOnly = true;
        column.ValueAccessor = accessor;
        column.ValueType = typeof(string);
        column.ShowFilterButton = true;
        column.Options = new DataGridColumnDefinitionOptions
        {
            SortValueAccessor = accessor, SortValueComparer = StringComparer.OrdinalIgnoreCase,
            FilterValueAccessor = accessor, IsSearchable = true, SearchTextProvider = Name
        };
        _grid.ColumnDefinitionsSource = new AvaloniaList<DataGridColumnDefinition> { column };
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "ProDataGrid collection projection uses explicit value accessors; no property-name reflection is used.")]
    private void Rebuild()
    {
        if (_grid is null || AssociatedObject is not { } browser) return;
        _generation++;
        DetachSource();
        _sync = true;
        try
        {
            _grid.ItemsSource = null;
            if (_view is not null) _view.CollectionChanged -= OnViewChanged;
            if ((object?)_view is IDisposable disposable) disposable.Dispose();
            IEnumerable items = browser.Items ?? Array.Empty<object>();
            _view = new DataGridCollectionView(items);
            _view.CollectionChanged += OnViewChanged;
            _grid.ItemsSource = _view;
            _source = items as INotifyCollectionChanged;
            if (_source is not null) _source.CollectionChanged += OnCollectionChanged;
            foreach (object? item in items)
                if (item is INotifyPropertyChanged observable && !_observed.Contains(observable))
                {
                    _observed.Add(observable);
                    observable.PropertyChanged += OnItemChanged;
                }
            ApplyProjection();
        }
        finally { _sync = false; }
    }

    private void ApplyProjection()
    {
        if (_grid is null || _view is null || AssociatedObject is not { } browser) return;
        bool wasSync = _sync;
        _sync = true;
        try
        {
            string query = browser.Query?.Trim() ?? string.Empty;
            if (query.Length == 0) { _filtering.Clear(); _search.Clear(); }
            else
            {
                _filtering.SetOrUpdate(new FilteringDescriptor(ColumnKey, FilteringOperator.Custom,
                    string.Empty, query, Array.Empty<object>(), item => Name(item).Contains(query, StringComparison.OrdinalIgnoreCase),
                    CultureInfo.CurrentCulture, StringComparison.OrdinalIgnoreCase));
                _search.Apply(new[] { new SearchDescriptor(query, SearchMatchMode.Contains, SearchTermCombineMode.Any,
                    SearchScope.VisibleColumns, Array.Empty<object>(), StringComparison.OrdinalIgnoreCase,
                    CultureInfo.CurrentCulture, wholeWord: false, normalizeWhitespace: true, ignoreDiacritics: false, allowEmpty: false) });
            }
            if (browser.SortIndex == 0) _sorting.Clear();
            else _sorting.Apply(new[] { new SortingDescriptor(ColumnKey,
                browser.SortIndex == 1 ? ListSortDirection.Ascending : ListSortDirection.Descending) });
            _grid.SelectedItem = browser.SelectedItem;
            browser.ResultCount = _view.Count;
        }
        finally { _sync = wasSync; }
    }

    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (_grid is null || AssociatedObject is not { } browser) return;
        if (e.Property == StudioAssetBrowser.ItemsProperty) Rebuild();
        else if (e.Property == StudioAssetBrowser.QueryProperty || e.Property == StudioAssetBrowser.SortIndexProperty) ApplyProjection();
        else if (e.Property == StudioAssetBrowser.ItemTemplateProperty) { ConfigureColumn(); ApplyProjection(); }
        else if (e.Property == StudioAssetBrowser.IsCompactProperty) _grid.RowHeight = browser.IsCompact ? 36 : 64;
        else if (e.Property == StudioAssetBrowser.SelectedItemProperty && !_sync)
        {
            _sync = true;
            try { _grid.SelectedItem = browser.SelectedItem; }
            finally { _sync = false; }
        }
    }
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        int version = _generation;
        Dispatcher.UIThread.Post(() => { if (_grid is not null && version == _generation) Rebuild(); });
    }
    private void OnItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ViewModelBase.Name) or nameof(ImageKeyViewModel.Key) or null or "")
        {
            int version = _generation;
            Dispatcher.UIThread.Post(() => { if (_grid is not null && version == _generation) { bool previous = _sync; _sync = true; try { _view?.Refresh(); ApplyProjection(); } finally { _sync = previous; } } });
        }
    }
    private void OnViewChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_view is not null && AssociatedObject is { } browser) browser.ResultCount = _view.Count;
    }
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // A filtered-out selection remains the document selection, not a request to clear it.
        // Explicit model clears still flow through OnChanged.
        if (!_sync && AssociatedObject is { } browser && _grid?.SelectedItem is { } selected)
            browser.SelectedItem = selected;
    }
    private static bool IsTextInput(object? source) => source is Visual visual &&
        (visual is TextBox or Button || visual.GetVisualAncestors().Any(x => x is TextBox or Button));
    private void OnOpen(object? sender, TappedEventArgs e)
    {
        if (!IsTextInput(e.Source) && ExecuteOpen()) e.Handled = true;
    }
    private void OnKey(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && !IsTextInput(e.Source) && ExecuteOpen()) e.Handled = true;
    }
    private bool ExecuteOpen()
    {
        if (AssociatedObject is not { SelectedItem: { } item, OpenCommand: { } command } || !command.CanExecute(item)) return false;
        command.Execute(item); return true;
    }
    private static string Name(object? item) => item is ImageKeyViewModel image ? image.Key ?? string.Empty : GridTextHelper.GetDisplayText(item);
}
