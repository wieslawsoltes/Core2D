// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using Avalonia;
using Avalonia.Controls;
using System.Collections;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Xaml.Interactivity;
using Core2D.Behaviors;

namespace Core2D.Controls.Studio;

/// <summary>A searchable, sortable ProDataGrid-backed asset browser without copied document state.</summary>
public class StudioAssetBrowser : TemplatedControl
{
    /// <summary>Gets or sets the original asset collection.</summary>
    public static readonly DirectProperty<StudioAssetBrowser, IEnumerable?> ItemsProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetBrowser, IEnumerable?>(nameof(Items), x => x.Items, (x, value) => x.Items = value);
    private IEnumerable? _items = null;
    /// <summary>Gets or sets the original asset collection.</summary>
    public IEnumerable? Items { get => _items; set => SetAndRaise(ItemsProperty, ref _items, value); }

    /// <summary>Gets or sets the selected original model object.</summary>
    public static readonly DirectProperty<StudioAssetBrowser, object?> SelectedItemProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetBrowser, object?>(nameof(SelectedItem), x => x.SelectedItem, (x, value) => x.SelectedItem = value);
    private object? _selectedItem = null;
    /// <summary>Gets or sets the selected original model object.</summary>
    public object? SelectedItem { get => _selectedItem; set => SetAndRaise(SelectedItemProperty, ref _selectedItem, value); }

    /// <summary>Defines the search input's contextual prompt.</summary>
    public static readonly DirectProperty<StudioAssetBrowser, string> SearchWatermarkProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetBrowser, string>(nameof(SearchWatermark), x => x.SearchWatermark, (x, value) => x.SearchWatermark = value);
    private string _searchWatermark = "Search assets…";
    /// <summary>Gets or sets the prompt without changing the browser's search semantics.</summary>
    public string SearchWatermark { get => _searchWatermark; set => SetAndRaise(SearchWatermarkProperty, ref _searchWatermark, value); }

    /// <summary>Gets or sets the compiled asset cell template.</summary>
    public static readonly DirectProperty<StudioAssetBrowser, IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetBrowser, IDataTemplate?>(nameof(ItemTemplate), x => x.ItemTemplate, (x, value) => x.ItemTemplate = value);
    private IDataTemplate? _itemTemplate = null;
    /// <summary>Gets or sets the compiled asset cell template.</summary>
    public IDataTemplate? ItemTemplate { get => _itemTemplate; set => SetAndRaise(ItemTemplateProperty, ref _itemTemplate, value); }

    /// <summary>Gets or sets the case-insensitive name filter.</summary>
    public static readonly DirectProperty<StudioAssetBrowser, string?> QueryProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetBrowser, string?>(nameof(Query), x => x.Query, (x, value) => x.Query = value);
    private string? _query = null;
    /// <summary>Gets or sets the case-insensitive name filter.</summary>
    public string? Query { get => _query; set => SetAndRaise(QueryProperty, ref _query, value); }

    /// <summary>Gets or sets compact row presentation.</summary>
    public static readonly DirectProperty<StudioAssetBrowser, bool> IsCompactProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetBrowser, bool>(nameof(IsCompact), x => x.IsCompact, (x, value) => x.IsCompact = value);
    private bool _isCompact = false;
    /// <summary>Gets or sets compact row presentation.</summary>
    public bool IsCompact { get => _isCompact; set => SetAndRaise(IsCompactProperty, ref _isCompact, value); }

    /// <summary>Gets or sets library order, name ascending, or name descending.</summary>
    public static readonly DirectProperty<StudioAssetBrowser, int> SortIndexProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetBrowser, int>(nameof(SortIndex), x => x.SortIndex, (x, value) => x.SortIndex = value);
    private int _sortIndex = 0;
    /// <summary>Gets or sets library order, name ascending, or name descending.</summary>
    public int SortIndex { get => _sortIndex; set => SetAndRaise(SortIndexProperty, ref _sortIndex, value); }

    /// <summary>Gets or sets the empty-library title.</summary>
    public static readonly DirectProperty<StudioAssetBrowser, string> EmptyTitleProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetBrowser, string>(nameof(EmptyTitle), x => x.EmptyTitle, (x, value) => x.EmptyTitle = value);
    private string _emptyTitle = "No assets yet";
    /// <summary>Gets or sets the empty-library title.</summary>
    public string EmptyTitle { get => _emptyTitle; set => SetAndRaise(EmptyTitleProperty, ref _emptyTitle, value); }

    /// <summary>Gets or sets empty-library guidance.</summary>
    public static readonly DirectProperty<StudioAssetBrowser, string> EmptyDescriptionProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetBrowser, string>(nameof(EmptyDescription), x => x.EmptyDescription, (x, value) => x.EmptyDescription = value);
    private string _emptyDescription = "Add an asset using the toolbar above.";
    /// <summary>Gets or sets empty-library guidance.</summary>
    public string EmptyDescription { get => _emptyDescription; set => SetAndRaise(EmptyDescriptionProperty, ref _emptyDescription, value); }

    /// <summary>Gets or sets the command activated on Enter or double-click.</summary>
    public static readonly DirectProperty<StudioAssetBrowser, ICommand?> OpenCommandProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetBrowser, ICommand?>(nameof(OpenCommand), x => x.OpenCommand, (x, value) => x.OpenCommand = value);
    private ICommand? _openCommand = null;
    /// <summary>Gets or sets the command activated on Enter or double-click.</summary>
    public ICommand? OpenCommand { get => _openCommand; set => SetAndRaise(OpenCommandProperty, ref _openCommand, value); }

    /// <summary>Gets or sets the filtered result count supplied by the browser behavior.</summary>
    public static readonly DirectProperty<StudioAssetBrowser, int> ResultCountProperty =
        AvaloniaProperty.RegisterDirect<StudioAssetBrowser, int>(nameof(ResultCount), x => x.ResultCount, (x, value) => x.ResultCount = value);
    private int _resultCount = 0;
    /// <summary>Gets or sets the filtered result count supplied by the browser behavior.</summary>
    public int ResultCount { get => _resultCount; set => SetAndRaise(ResultCountProperty, ref _resultCount, value); }

    /// <summary>Initializes the browser's grid and lifecycle behavior.</summary>
    public StudioAssetBrowser() => Interaction.GetBehaviors(this).Add(new StudioAssetBrowserBehavior());
}
