// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.DataGridFiltering;
using Avalonia.Controls.DataGridSearching;
using Avalonia.Controls.DataGridSorting;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;
using Core2D.ViewModels.Editor;

namespace Core2D.Behaviors;

/// <summary>Projects declarative actions into a native grid, restores focus, and executes only explicit user choices.</summary>
public sealed class StudioCommandPaletteBehavior : Behavior<StudioCommandPalette>
{
    private readonly List<(MenuItem Menu, StudioActionViewModel Action, bool ParentEnabled)> _catalog = new();
    private readonly List<MenuItem> _observedMenus = new();
    private readonly HashSet<ICommand> _commands = new(ReferenceEqualityComparer.Instance);
    private readonly List<string> _recent = new();
    private List<StudioActionViewModel> _results = new();
    private DataGrid? _grid;
    private TextBox? _search;
    private Border? _surface;
    private IInputElement? _previousFocus;
    private DataGridCollectionView? _view;
    private bool _pending;
    private int _generation;

    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } palette) return;
        palette.TemplateApplied += OnTemplate;
        palette.PropertyChanged += OnChanged;
        palette.Loaded += OnLoaded;
        palette.AddHandler(InputElement.KeyDownEvent, OnKey, RoutingStrategies.Tunnel);
        palette.AddHandler(InputElement.PointerPressedEvent, OnPointer, RoutingStrategies.Tunnel);
        palette.AddHandler(Button.ClickEvent, OnClick);
        Connect(palette.GetVisualDescendants().OfType<DataGrid>().FirstOrDefault(x => x.Name == "PART_Grid" && ReferenceEquals(x.TemplatedParent, palette)),
            palette.GetVisualDescendants().OfType<TextBox>().FirstOrDefault(x => x.Name == "PART_Search" && ReferenceEquals(x.TemplatedParent, palette)),
            palette.GetVisualDescendants().OfType<Border>().FirstOrDefault(x => x.Name == "PART_Surface" && ReferenceEquals(x.TemplatedParent, palette)));
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } palette)
        {
            palette.TemplateApplied -= OnTemplate;
            palette.PropertyChanged -= OnChanged;
            palette.Loaded -= OnLoaded;
            palette.RemoveHandler(InputElement.KeyDownEvent, OnKey);
            palette.RemoveHandler(InputElement.PointerPressedEvent, OnPointer);
            palette.RemoveHandler(Button.ClickEvent, OnClick);
        }
        ClearCatalog();
        DisconnectGrid();
        RestoreFocus();
        base.OnDetaching();
    }
    private void OnTemplate(object? sender, TemplateAppliedEventArgs e) => Connect(
        e.NameScope.Find<DataGrid>("PART_Grid"), e.NameScope.Find<TextBox>("PART_Search"), e.NameScope.Find<Border>("PART_Surface"));
    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject?.IsOpen == true) Open();
    }
    private void Connect(DataGrid? grid, TextBox? search, Border? surface)
    {
        DisconnectGrid();
        _grid = grid; _search = search; _surface = surface;
        if (_grid is null) return;
        var accessor = new DataGridColumnValueAccessor<StudioActionViewModel, string>(x => x.Title, (_, _) => { });
        _grid.ColumnDefinitionsSource = new AvaloniaList<DataGridColumnDefinition>
        {
            new DataGridTemplateColumnDefinition
            {
                Header = "Action", ColumnKey = "Action", CellTemplateKey = "StudioActionCell", IsReadOnly = true,
                Width = new DataGridLength(1, DataGridLengthUnitType.Star), ValueAccessor = accessor, ValueType = typeof(string),
                Options = new DataGridColumnDefinitionOptions { IsSearchable = true, SearchTextProvider = x => x is StudioActionViewModel action ? action.Title + " " + action.Category : "", SortValueAccessor = accessor, FilterValueAccessor = accessor }
            }
        };
        _grid.FilteringModel = new FilteringModel { OwnsViewFilter = true };
        _grid.FilteringAdapterFactory = new DataGridAccessorFilteringAdapterFactory();
        _grid.SortingModel = new SortingModel { OwnsViewSorts = true };
        _grid.SearchModel = new SearchModel();
        _grid.SearchAdapterFactory = new DataGridAccessorSearchAdapterFactory();
        _grid.DoubleTapped += OnDoubleTapped;
        if (AssociatedObject?.IsOpen == true) Open();
    }
    private void DisconnectGrid()
    {
        if (_grid is not null)
        {
            _grid.DoubleTapped -= OnDoubleTapped;
            _grid.ItemsSource = null;
            _grid.ColumnDefinitionsSource = null;
        }
        DisposeView();
        _grid = null; _search = null; _surface = null;
    }
    private void DisposeView()
    {
        if ((object?)_view is IDisposable disposable) disposable.Dispose();
        _view = null;
    }
    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == StudioCommandPalette.IsOpenProperty)
        {
            if (AssociatedObject?.IsOpen == true) Open();
            else { ClearCatalog(); if (_grid is not null) _grid.ItemsSource = null; DisposeView(); RestoreFocus(); }
        }
        else if (e.Property == StudioCommandPalette.QueryProperty) Filter();
        else if (e.Property == StyledElement.DataContextProperty && AssociatedObject?.IsOpen == true) { RebuildCatalog(); Filter(); }
        else if (e.Property == Visual.BoundsProperty) Constrain();
    }
    private void Open()
    {
        if (_grid is null || AssociatedObject is not { } palette || palette.GetVisualRoot() is null) return;
        IInputElement? focused = TopLevel.GetTopLevel(palette)?.FocusManager?.GetFocusedElement();
        if (focused is not Visual visual || !visual.GetVisualAncestors().Contains(palette)) _previousFocus = focused;
        RebuildCatalog();
        Filter();
        Constrain();
        int generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (generation != _generation || palette.IsOpen != true) return;
            _search?.Focus(); _search?.SelectAll();
        }, DispatcherPriority.Input);
    }
    private void Constrain()
    {
        if (_surface is null || AssociatedObject is not { } palette) return;
        _surface.MaxWidth = Math.Max(0, palette.Bounds.Width - 32);
        _surface.MaxHeight = Math.Max(0, palette.Bounds.Height - 32);
    }
    private void RestoreFocus()
    {
        IInputElement? focused = AssociatedObject is { } palette ? TopLevel.GetTopLevel(palette)?.FocusManager?.GetFocusedElement() : null;
        if (focused is null || focused is Visual visual && AssociatedObject is { } owner && visual.GetVisualAncestors().Contains(owner))
            if (_previousFocus is InputElement { IsEffectivelyEnabled: true, IsEffectivelyVisible: true } previous) previous.Focus();
        _previousFocus = null;
    }
    private void ClearCatalog()
    {
        _generation++; _pending = false;
        foreach (MenuItem menu in _observedMenus) menu.PropertyChanged -= OnMenuChanged;
        foreach (ICommand command in _commands) command.CanExecuteChanged -= OnAvailabilityChanged;
        _commands.Clear(); _observedMenus.Clear(); _catalog.Clear(); _results.Clear();
    }
    private void RebuildCatalog()
    {
        ClearCatalog();
        if (AssociatedObject is not { } palette) return;
        foreach (object? item in palette.Items) if (item is MenuItem menu) Visit(menu, "", true, 0);
    }
    private void Visit(MenuItem menu, string category, bool parentEnabled, int depth)
    {
        if (depth > 8 || _catalog.Count >= 512 || _observedMenus.Contains(menu)) return;
        _observedMenus.Add(menu);
        menu.PropertyChanged += OnMenuChanged;
        if (!menu.IsVisible) return;
        string title = (menu.Header as string ?? "").Replace("__", "\0").Replace("_", "").Replace("\0", "_");
        bool enabled = parentEnabled && menu.IsEnabled;
        if (menu.Command is { } command)
        {
            var action = new StudioActionViewModel(title, category, menu.InputGesture?.ToString() ?? "", command, menu.CommandParameter);
            action.RefreshAvailability(enabled);
            _catalog.Add((menu, action, parentEnabled));
            if (_commands.Add(command)) command.CanExecuteChanged += OnAvailabilityChanged;
        }
        string path = category.Length == 0 ? title : category + " / " + title;
        foreach (object? child in menu.Items) if (child is MenuItem nested) Visit(nested, path, enabled, depth + 1);
    }
    private void OnMenuChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == MenuItem.CommandProperty || e.Property == MenuItem.CommandParameterProperty || e.Property == HeaderedSelectingItemsControl.HeaderProperty
            || e.Property == InputElement.IsEnabledProperty || e.Property == Visual.IsVisibleProperty) QueueRefresh();
    }
    private void OnAvailabilityChanged(object? sender, EventArgs e) => QueueRefresh();
    private void QueueRefresh()
    {
        // Command implementations may raise notifications off-thread. Never touch controls there.
        Dispatcher.UIThread.Post(() =>
        {
            if (_pending || AssociatedObject?.IsOpen != true || _grid is null) return;
            _pending = true;
            int version = _generation;
            Dispatcher.UIThread.Post(() =>
            {
                if (version != _generation || AssociatedObject?.IsOpen != true) return;
                _pending = false;
                RebuildCatalog(); Filter();
            });
        });
    }
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "The native grid has explicit accessors and compiled row templates; no reflection-based column discovery is used.")]
    private void Filter()
    {
        if (_grid is null || AssociatedObject is not { IsOpen: true } palette) return;
        string? previous = (_grid.SelectedItem as StudioActionViewModel)?.Id;
        var actions = new List<StudioActionViewModel>();
        foreach (var entry in _catalog) if (entry.Action.Matches(palette.Query)) actions.Add(entry.Action);
        // Stable history ordering stores strings only, so prior projects are never retained by recent actions.
        actions = actions.OrderBy(x =>
        {
            int index = _recent.IndexOf(x.Id);
            return string.IsNullOrWhiteSpace(palette.Query) && index >= 0 ? index : 8;
        }).ThenBy(x => x.IsAvailable ? 0 : 1).ToList();
        _results = actions;
        _grid.ItemsSource = null;
        DisposeView();
        _view = new DataGridCollectionView(actions);
        _grid.ItemsSource = _view;
        _grid.SelectedItem = actions.FirstOrDefault(x => x.Id == previous && x.IsAvailable) ?? actions.FirstOrDefault(x => x.IsAvailable) ?? actions.FirstOrDefault();
        palette.ResultCount = actions.Count;
    }
    private void OnKey(object? sender, KeyEventArgs e)
    {
        if (AssociatedObject?.IsOpen != true) return;
        if (_search?.GetVisualDescendants().OfType<TextPresenter>().Any(x => !string.IsNullOrEmpty(x.PreeditText)) == true) return;
        if (e.Key == Key.Escape) { AssociatedObject.IsOpen = false; e.Handled = true; }
        else if (e.Key is Key.Up or Key.Down)
        {
            int index = _grid?.SelectedIndex ?? -1;
            int step = e.Key == Key.Down ? 1 : -1;
            for (int next = index + step; next >= 0 && next < _results.Count; next += step)
                if (_results[next].IsAvailable) { if (_grid is not null) { _grid.SelectedItem = _results[next]; _grid.ScrollIntoView(_results[next], null); } break; }
            e.Handled = true;
        }
        else if (e.Key == Key.Enter) { ExecuteSelected(); e.Handled = true; }
    }
    private void OnDoubleTapped(object? sender, TappedEventArgs e) { ExecuteSelected(); e.Handled = true; }
    private void OnClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source is Button { Name: "PART_Close" } && AssociatedObject is { } palette) { palette.IsOpen = false; e.Handled = true; }
    }
    private void OnPointer(object? sender, PointerPressedEventArgs e)
    {
        if (_surface is not null && !new Rect(_surface.Bounds.Size).Contains(e.GetPosition(_surface)) && AssociatedObject is { } palette)
        { palette.IsOpen = false; e.Handled = true; }
    }
    private void ExecuteSelected()
    {
        if (_grid?.SelectedItem is not StudioActionViewModel action || AssociatedObject is not { } palette) return;
        var entry = _catalog.FirstOrDefault(x => ReferenceEquals(x.Action, action));
        if (entry.Menu is null || !entry.ParentEnabled || !entry.Menu.IsEnabled || !entry.Menu.IsVisible || !action.Command.CanExecute(action.Parameter)) return;
        _recent.Remove(action.Id); _recent.Insert(0, action.Id);
        if (_recent.Count > 8) _recent.RemoveAt(8);
        // Closing restores the prior canvas focus before a command opens a dialog or manipulates selection.
        palette.IsOpen = false;
        action.Command.Execute(action.Parameter);
    }
}
