// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Core2D.Model.Editor;
using Core2D.Model.History;
using ReactiveUI;

namespace Core2D.ViewModels.Editor;

/// <summary>Session-scoped page guides with atomic document-history operations.</summary>
public sealed class CanvasGuidesViewModel : ReactiveObject
{
    private readonly ObservableCollection<CanvasGuide> _items = new();
    private readonly IHistory? _history;
    private bool _isVisible = true, _isLocked;

    /// <summary>Creates guides sharing the owning document's history.</summary>
    public CanvasGuidesViewModel(IHistory? history = null)
    {
        _history = history;
        Items = new ReadOnlyObservableCollection<CanvasGuide>(_items);
    }
    /// <summary>Gets the guides, mutable only through validated operations.</summary>
    public ReadOnlyObservableCollection<CanvasGuide> Items { get; }
    /// <summary>Gets or sets guide visibility without deleting guide data.</summary>
    public bool IsVisible { get => _isVisible; set => this.RaiseAndSetIfChanged(ref _isVisible, value); }
    /// <summary>Gets or sets whether guide editing is locked.</summary>
    public bool IsLocked { get => _isLocked; set => this.RaiseAndSetIfChanged(ref _isLocked, value); }
    /// <summary>Adds a guide, returning its stable identity, or null when invalid/locked/full.</summary>
    public Guid? Add(bool vertical, double position)
    {
        if (IsLocked || !Valid(position) || _items.Count >= 512) return null;
        var guide = new CanvasGuide(Guid.NewGuid(), vertical, position);
        Apply(_items.Append(guide).ToArray());
        return guide.Id;
    }
    /// <summary>Moves an existing guide with one undo record.</summary>
    public bool Move(Guid id, double position)
    {
        if (IsLocked || !Valid(position)) return false;
        CanvasGuide[] next = _items.ToArray();
        int index = Array.FindIndex(next, x => x.Id == id);
        if (index < 0 || next[index].Position == position) return false;
        next[index] = next[index] with { Position = position };
        Apply(next);
        return true;
    }
    /// <summary>Removes a guide by stable identity.</summary>
    public bool Remove(Guid id)
    {
        if (IsLocked || !_items.Any(x => x.Id == id)) return false;
        Apply(_items.Where(x => x.Id != id).ToArray());
        return true;
    }
    /// <summary>Clears all guides as one undoable operation.</summary>
    public void Clear()
    {
        if (!IsLocked && _items.Count != 0) Apply(Array.Empty<CanvasGuide>());
    }
    private static bool Valid(double value) => double.IsFinite(value) && Math.Abs(value) <= 1e15;
    private void Apply(CanvasGuide[] next)
    {
        CanvasGuide[] previous = _items.ToArray();
        _history?.Snapshot(previous, next, Restore);
        Restore(next);
    }
    private void Restore(CanvasGuide[] values)
    {
        _items.Clear();
        foreach (CanvasGuide value in values) _items.Add(value);
    }
}
