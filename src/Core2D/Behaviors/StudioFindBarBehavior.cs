// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.DataGridSearching;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Uses the target's native search model; restores only search state still owned by this bar.</summary>
public sealed class StudioFindBarBehavior : Behavior<StudioFindBar>
{
    private DataGrid? _grid;
    private ISearchModel? _search;
    private SearchDescriptor[] _previous = Array.Empty<SearchDescriptor>();
    private SearchDescriptor? _owned;
    private bool _applying;
    private bool OwnsSearch => _owned is not null && _search?.Descriptors.Count == 1 && ReferenceEquals(_search.Descriptors[0], _owned);
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } bar) return;
        bar.PropertyChanged += OnChanged;
        bar.AddHandler(Button.ClickEvent, OnClick);
        bar.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        Connect();
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } bar)
        {
            bar.PropertyChanged -= OnChanged;
            bar.RemoveHandler(Button.ClickEvent, OnClick);
            bar.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
        }
        Disconnect();
        base.OnDetaching();
    }
    private void OnChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == StudioFindBar.TargetProperty) Connect();
        else if (e.Property == StudioFindBar.QueryProperty) Apply();
    }
    private void Connect()
    {
        Disconnect();
        _grid = AssociatedObject?.Target;
        if (_grid is not null) _grid.PropertyChanged += OnGridChanged;
        ConnectSearch();
    }
    private void OnGridChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property.Name == nameof(DataGrid.SearchModel)) ConnectSearch();
        else if (e.Property.Name == nameof(DataGrid.HierarchicalModel)) Refresh();
    }
    private void ReleaseSearch()
    {
        if (_search is not null)
        {
            _search.SearchChanged -= OnSearchChanged;
            _search.ResultsChanged -= OnResults;
            _search.CurrentChanged -= OnCurrent;
            if (OwnsSearch) _search.Apply(_previous);
        }
        _search = null; _owned = null; _previous = Array.Empty<SearchDescriptor>();
    }
    private void Disconnect()
    {
        if (_grid is not null) _grid.PropertyChanged -= OnGridChanged;
        ReleaseSearch(); _grid = null;
    }
    private void ConnectSearch()
    {
        ReleaseSearch();
        _search = _grid?.SearchModel;
        if (_search is not null)
        {
            _previous = _search.Descriptors.ToArray();
            _search.SearchChanged += OnSearchChanged;
            _search.ResultsChanged += OnResults;
            _search.CurrentChanged += OnCurrent;
        }
        Apply();
    }
    private void Apply()
    {
        string query = AssociatedObject?.Query?.Trim() ?? "";
        if (_search is not null)
        {
            _applying = true;
            try
            {
                if (query.Length == 0)
                {
                    bool restore = OwnsSearch;
                    _owned = null;
                    if (restore) _search.Apply(_previous);
                }
                else
                {
                    if (!OwnsSearch) _previous = _search.Descriptors.ToArray();
                    SearchDescriptor descriptor = new(query, SearchMatchMode.Contains, SearchTermCombineMode.All,
                        SearchScope.VisibleColumns, comparison: StringComparison.OrdinalIgnoreCase);
                    _search.Apply(new[] { descriptor });
                    // Equal descriptors may be retained by SearchModel rather than replaced.
                    _owned = _search.Descriptors.Count == 1 && _search.Descriptors[0].Equals(descriptor) ? _search.Descriptors[0] : null;
                }
            }
            finally { _applying = false; }
        }
        Refresh();
    }
    private void OnSearchChanged(object? sender, SearchChangedEventArgs e)
    {
        if (_applying || OwnsSearch) return;
        // Another client now owns the grid's query. Do not navigate it or restore over it.
        _owned = null;
        _previous = _search?.Descriptors.ToArray() ?? Array.Empty<SearchDescriptor>();
        Refresh();
    }
    private void OnResults(object? sender, SearchResultsChangedEventArgs e) => Refresh();
    private void OnCurrent(object? sender, SearchCurrentChangedEventArgs e) => Refresh();
    private void Refresh() => AssociatedObject?.Update(!OwnsSearch ? 0 : _search?.Results.Count ?? 0,
        _search?.CurrentIndex ?? -1, _grid?.HierarchicalModel is not null);
    private void Navigate(bool previous)
    {
        if (_search is null || !OwnsSearch || AssociatedObject?.IsEffectivelyEnabled != true) return;
        bool selection = _search.UpdateSelectionOnNavigate;
        try
        {
            _search.UpdateSelectionOnNavigate = true;
            bool moved = previous ? _search.MovePrevious() : _search.MoveNext();
            if (!moved && _grid is not null && _search.CurrentResult is { } result)
            {
                // A single match has no new index to publish, but Enter must still reveal it.
                _grid.SelectedItem = result.Item;
                _grid.ScrollIntoView(result.Item, null);
            }
        }
        finally { _search.UpdateSelectionOnNavigate = selection; }
        Refresh();
    }
    private void OnClick(object? sender, RoutedEventArgs e)
    {
        if (AssociatedObject?.IsEffectivelyEnabled != true || e.Source is not Button button) return;
        switch (button.Name)
        {
            case "PART_Previous": Navigate(true); break;
            case "PART_Next": Navigate(false); break;
            case "PART_ExpandAll": _grid?.HierarchicalModel?.ExpandAll(); break;
            case "PART_CollapseAll": _grid?.HierarchicalModel?.CollapseAll(); break;
            default: return;
        }
        e.Handled = true;
    }
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (AssociatedObject is not { } bar || e.Source is not TextBox input || !bar.IsEffectivelyEnabled) return;
        if (input.GetVisualDescendants().OfType<TextPresenter>().Any(x => !string.IsNullOrEmpty(x.PreeditText))) return;
        if (e.Key == Key.Enter) { Navigate(e.KeyModifiers.HasFlag(KeyModifiers.Shift)); e.Handled = true; }
        else if (e.Key == Key.Escape && !string.IsNullOrEmpty(bar.Query)) { bar.Query = ""; e.Handled = true; }
    }
}
