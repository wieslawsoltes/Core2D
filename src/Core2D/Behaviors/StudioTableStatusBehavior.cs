// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System.Collections;
using System.Collections.Specialized;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using Core2D.Controls.Studio;

namespace Core2D.Behaviors;

/// <summary>Observes filtered view counts and selection without altering grid filtering or source order.</summary>
public sealed class StudioTableStatusBehavior : Behavior<StudioTableStatus>
{
    private DataGrid? _grid;
    private INotifyCollectionChanged? _collection;
    private int _generation;
    private bool _pending;
    /// <inheritdoc />
    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject is not { } status) return;
        status.PropertyChanged += OnStatusChanged;
        Connect();
    }
    /// <inheritdoc />
    protected override void OnDetaching()
    {
        if (AssociatedObject is { } status) status.PropertyChanged -= OnStatusChanged;
        Disconnect();
        base.OnDetaching();
    }
    private void OnStatusChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == StudioTableStatus.TargetProperty) Connect();
        else if (e.Property == StudioTableStatus.TotalCountProperty || e.Property == StudioTableStatus.UnitProperty) Refresh();
    }
    private void Connect()
    {
        Disconnect();
        _grid = AssociatedObject?.Target;
        if (_grid is not null)
        {
            _grid.PropertyChanged += OnGridChanged;
            _grid.SelectionChanged += OnSelectionChanged;
        }
        ObserveCollection();
    }
    private void Disconnect()
    {
        _generation++; _pending = false;
        if (_grid is not null)
        {
            _grid.PropertyChanged -= OnGridChanged;
            _grid.SelectionChanged -= OnSelectionChanged;
        }
        if (_collection is not null) _collection.CollectionChanged -= OnCollectionChanged;
        _collection = null; _grid = null;
    }
    private void OnGridChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == DataGrid.ItemsSourceProperty) ObserveCollection();
    }
    private void ObserveCollection()
    {
        if (_collection is not null) _collection.CollectionChanged -= OnCollectionChanged;
        _collection = _grid?.ItemsSource as INotifyCollectionChanged;
        if (_collection is not null) _collection.CollectionChanged += OnCollectionChanged;
        Refresh();
    }
    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e) => Refresh();
    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) => Schedule();
    private void Schedule()
    {
        if (_pending) return;
        _pending = true;
        int generation = _generation;
        Dispatcher.UIThread.Post(() =>
        {
            if (generation != _generation) return;
            _pending = false; Refresh();
        }, DispatcherPriority.Background);
    }
    private void Refresh()
    {
        int count = _grid?.ItemsSource switch
        {
            null => 0,
            DataGridCollectionView view => view.Count,
            ICollection collection => collection.Count,
            _ => -1
        };
        AssociatedObject?.Update(count, _grid?.SelectedItems?.Count ?? 0);
    }
}
