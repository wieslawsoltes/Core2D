// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using Core2D.Model.History;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using ReactiveUI;

namespace Core2D.ViewModels.Editor;

/// <summary>Edits single or mixed shape-state bits against original objects with one undo record.</summary>
public sealed class ShapeFlagsInspectorViewModel : ReactiveObject, IDisposable
{
    private readonly Dictionary<BaseShapeViewModel, ShapeStateFlags> _states = new(ReferenceEqualityComparer.Instance);
    private readonly int[] _enabledCounts = new int[32];
    private readonly IHistory? _history;
    private bool _disposed, _batch;

    /// <summary>Observes distinct original shapes; collection membership is owned by the hosting control.</summary>
    public ShapeFlagsInspectorViewModel(IEnumerable<BaseShapeViewModel> shapes, IHistory? history = null)
    {
        ArgumentNullException.ThrowIfNull(shapes);
        _history = history;
        foreach (BaseShapeViewModel shape in shapes)
        {
            if (shape is null || !_states.TryAdd(shape, shape.State)) continue;
            CountBits((uint)shape.State, 1);
            shape.PropertyChanged += OnChanged;
        }
    }

    /// <summary>Gets the number of distinct shapes, not duplicated references.</summary>
    public int Count => _states.Count;
    /// <summary>Gets whether this live adapter has any editable targets.</summary>
    public bool CanEdit => !_disposed && Count > 0;
    /// <summary>Gets a concise selection identity.</summary>
    public string Title
    {
        get
        {
            if (Count == 0) return "No object selected";
            if (Count > 1) return $"{Count} objects";
            foreach (BaseShapeViewModel shape in _states.Keys) return string.IsNullOrWhiteSpace(shape.Name) ? "Object state" : shape.Name;
            return "Object state";
        }
    }
    /// <summary>Gets the actual bit mask or an explicit mixed-state indicator; unknown bits are preserved.</summary>
    public string Mask
    {
        get
        {
            if (Count == 0) return "—";
            uint common = 0;
            for (int bit = 0; bit < 32; bit++)
            {
                if (_enabledCounts[bit] != 0 && _enabledCounts[bit] != Count) return "Mixed flags";
                if (_enabledCounts[bit] == Count) common |= 1u << bit;
            }
            return $"0x{common:X8}";
        }
    }
    /// <summary>Gets or sets visibility; null indicates mixed values and is never written.</summary>
    public bool? Visible { get => Read(ShapeStateFlags.Visible); set => Write(ShapeStateFlags.Visible, value); }
    /// <summary>Gets or sets printable state.</summary>
    public bool? Printable { get => Read(ShapeStateFlags.Printable); set => Write(ShapeStateFlags.Printable, value); }
    /// <summary>Gets or sets the editing lock; changing it never replaces other bits.</summary>
    public bool? Locked { get => Read(ShapeStateFlags.Locked); set => Write(ShapeStateFlags.Locked, value); }
    /// <summary>Gets or sets the renderer size flag.</summary>
    public bool? Size { get => Read(ShapeStateFlags.Size); set => Write(ShapeStateFlags.Size, value); }
    /// <summary>Gets or sets the renderer thickness flag.</summary>
    public bool? Thickness { get => Read(ShapeStateFlags.Thickness); set => Write(ShapeStateFlags.Thickness, value); }
    /// <summary>Gets or sets connector state.</summary>
    public bool? Connector { get => Read(ShapeStateFlags.Connector); set => Write(ShapeStateFlags.Connector, value); }
    /// <summary>Gets or sets the explicit None connector-role bit, which is not the zero mask.</summary>
    public bool? None { get => Read(ShapeStateFlags.None); set => Write(ShapeStateFlags.None, value); }
    /// <summary>Gets or sets standalone state.</summary>
    public bool? Standalone { get => Read(ShapeStateFlags.Standalone); set => Write(ShapeStateFlags.Standalone, value); }
    /// <summary>Gets or sets input-role state.</summary>
    public bool? Input { get => Read(ShapeStateFlags.Input); set => Write(ShapeStateFlags.Input, value); }
    /// <summary>Gets or sets output-role state.</summary>
    public bool? Output { get => Read(ShapeStateFlags.Output); set => Write(ShapeStateFlags.Output, value); }

    private bool? Read(ShapeStateFlags flag)
    {
        int enabled = _enabledCounts[BitOperations.TrailingZeroCount((uint)flag)];
        return Count == 0 ? null : enabled == 0 ? false : enabled == Count ? true : null;
    }
    private void Write(ShapeStateFlags flag, bool? value)
    {
        if (!CanEdit || value is null || value == Read(flag)) return;
        List<Entry> before = new(), after = new();
        foreach (BaseShapeViewModel shape in _states.Keys)
        {
            ShapeStateFlags previous = shape.State;
            ShapeStateFlags next = value.Value ? previous | flag : previous & ~flag;
            if (previous == next) continue;
            before.Add(new Entry(shape, previous)); after.Add(new Entry(shape, next));
        }
        if (after.Count == 0) return;
        Entry[] nextEntries = after.ToArray();
        _batch = true;
        try
        {
            _history?.Snapshot(before.ToArray(), nextEntries, Apply);
            Apply(nextEntries);
        }
        finally { _batch = false; NotifyFlags(); }
    }
    private static void Apply(Entry[] entries)
    {
        foreach (Entry entry in entries) entry.Shape.State = entry.State;
    }
    private void CountBits(uint value, int delta)
    {
        while (value != 0)
        {
            int bit = BitOperations.TrailingZeroCount(value);
            _enabledCounts[bit] += delta;
            value &= value - 1;
        }
    }
    private void OnChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_disposed || sender is not BaseShapeViewModel shape || !_states.TryGetValue(shape, out ShapeStateFlags previous)) return;
        if (e.PropertyName is nameof(BaseShapeViewModel.State) or null or "")
        {
            ShapeStateFlags current = shape.State;
            _states[shape] = current;
            CountBits((uint)previous & ~(uint)current, -1);
            CountBits((uint)current & ~(uint)previous, 1);
            if (!_batch) NotifyFlags();
        }
        if (e.PropertyName is nameof(BaseShapeViewModel.Name) or null or "") this.RaisePropertyChanged(nameof(Title));
    }
    private void NotifyFlags()
    {
        this.RaisePropertyChanged(nameof(Mask));
        this.RaisePropertyChanged(nameof(Visible)); this.RaisePropertyChanged(nameof(Printable));
        this.RaisePropertyChanged(nameof(Locked)); this.RaisePropertyChanged(nameof(Size));
        this.RaisePropertyChanged(nameof(Thickness)); this.RaisePropertyChanged(nameof(Connector));
        this.RaisePropertyChanged(nameof(None)); this.RaisePropertyChanged(nameof(Standalone));
        this.RaisePropertyChanged(nameof(Input)); this.RaisePropertyChanged(nameof(Output));
    }
    /// <summary>Releases subscriptions and makes stale adapters inert; recorded history remains valid.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (BaseShapeViewModel shape in _states.Keys) shape.PropertyChanged -= OnChanged;
        this.RaisePropertyChanged(nameof(CanEdit));
    }
    private readonly record struct Entry(BaseShapeViewModel Shape, ShapeStateFlags State);
}
