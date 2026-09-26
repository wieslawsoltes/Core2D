// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.ComponentModel;
using Core2D.Model.History;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using ReactiveUI;

namespace Core2D.ViewModels.Editor;

/// <summary>Observes one virtualized hierarchy row without duplicating document state.</summary>
public sealed class StudioLayerRowViewModel : ReactiveObject, IDisposable
{
    private readonly IHistory? _history;
    private bool _disposed;
    /// <summary>Creates an adapter for an existing project hierarchy item.</summary>
    public StudioLayerRowViewModel(ViewModelBase item, IHistory? history)
    {
        Item = item;
        _history = history;
        item.PropertyChanged += OnChanged;
    }
    /// <summary>Gets the original item; its identity is never replaced by row editing.</summary>
    public ViewModelBase Item { get; }
    /// <summary>Gets or sets the item name as one history operation.</summary>
    public string? Name
    {
        get => Item.Name;
        set
        {
            if (_disposed || string.IsNullOrWhiteSpace(value)) return;
            string next = value.Trim();
            if (next.Length > 256 || next == Item.Name) return;
            ViewModelBase item = Item;
            _history?.Snapshot(item.Name, next, text => item.Name = text);
            item.Name = next;
        }
    }
    /// <summary>Gets whether the item supports a shape lock.</summary>
    public bool CanLock => Item is BaseShapeViewModel;
    /// <summary>Gets whether visibility exists on the original model.</summary>
    public bool CanChangeVisibility => Item is BaseShapeViewModel or BaseContainerViewModel;
    /// <summary>Gets or sets shape/container visibility without discarding state bits.</summary>
    public bool IsVisible
    {
        get => Item is BaseShapeViewModel shape ? shape.State.HasFlag(ShapeStateFlags.Visible) : Item is not BaseContainerViewModel container || container.IsVisible;
        set
        {
            if (_disposed || value == IsVisible) return;
            if (Item is BaseShapeViewModel shape) SetFlag(shape, ShapeStateFlags.Visible, value);
            else if (Item is BaseContainerViewModel container)
            {
                _history?.Snapshot(container.IsVisible, value, visible => container.IsVisible = visible);
                container.IsVisible = value;
            }
        }
    }
    /// <summary>Gets or sets the shape lock. Containers do not fabricate a lock property.</summary>
    public bool IsLocked
    {
        get => Item is BaseShapeViewModel shape && shape.State.HasFlag(ShapeStateFlags.Locked);
        set { if (!_disposed && Item is BaseShapeViewModel shape && value != IsLocked) SetFlag(shape, ShapeStateFlags.Locked, value); }
    }
    private void SetFlag(BaseShapeViewModel shape, ShapeStateFlags flag, bool enabled)
    {
        ShapeStateFlags previous = shape.State, next = enabled ? previous | flag : previous & ~flag;
        _history?.Snapshot(previous, next, state => shape.State = state);
        shape.State = next;
    }
    private void OnChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(ViewModelBase.Name) or null) this.RaisePropertyChanged(nameof(Name));
        if (e.PropertyName is nameof(BaseShapeViewModel.State) or nameof(BaseContainerViewModel.IsVisible) or null)
        { this.RaisePropertyChanged(nameof(IsVisible)); this.RaisePropertyChanged(nameof(IsLocked)); }
    }
    /// <summary>Disconnects the row adapter; already-recorded undo operations remain valid.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        Item.PropertyChanged -= OnChanged;
    }
}
