// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.ComponentModel;
using Core2D.Model.History;
using Core2D.ViewModels.Shapes;
using ReactiveUI;

namespace Core2D.ViewModels.Editor;

/// <summary>Edits two-corner bounds, preserving point identity, orientation and one undo record per edit.</summary>
public sealed class BoundsInspectorViewModel : ReactiveObject, IDisposable
{
    private readonly PointShapeViewModel _start;
    private readonly PointShapeViewModel _end;
    private readonly IHistory? _history;
    private bool _isAspectLocked;
    private int _anchorIndex;
    private bool _updating;
    private bool _disposed;

    /// <summary>Creates an editor for existing corner points. The editor never replaces either point.</summary>
    public BoundsInspectorViewModel(PointShapeViewModel start, PointShapeViewModel end, IHistory? history = null)
    {
        _start = start ?? throw new ArgumentNullException(nameof(start));
        _end = end ?? throw new ArgumentNullException(nameof(end));
        _history = history;
        _start.PropertyChanged += OnPointChanged;
        if (!ReferenceEquals(_start, _end)) _end.PropertyChanged += OnPointChanged;
    }

    /// <summary>Gets whether the two distinct points have finite, safely editable coordinates.</summary>
    public bool IsValid => !ReferenceEquals(_start, _end) && Valid(_start.X) && Valid(_start.Y) && Valid(_end.X) && Valid(_end.Y);
    /// <summary>Gets or sets whether resizing preserves the current non-zero aspect ratio.</summary>
    public bool IsAspectLocked { get => _isAspectLocked; set => this.RaiseAndSetIfChanged(ref _isAspectLocked, value); }
    /// <summary>Gets or sets the left coordinate, translating both corners.</summary>
    public decimal X { get => IsValid ? Math.Min((decimal)_start.X, (decimal)_end.X) : 0; set { if (value >= -1e15m && value <= 1e15m) Translate(value - X, 0); } }
    /// <summary>Gets or sets the top coordinate, translating both corners.</summary>
    public decimal Y { get => IsValid ? Math.Min((decimal)_start.Y, (decimal)_end.Y) : 0; set { if (value >= -1e15m && value <= 1e15m) Translate(0, value - Y); } }
    /// <summary>Gets or sets the non-negative width, resizing about the selected anchor.</summary>
    public decimal Width { get => IsValid ? Math.Abs((decimal)_end.X - (decimal)_start.X) : 0; set => Resize(value, true); }
    /// <summary>Gets or sets the non-negative height, resizing about the selected anchor.</summary>
    public decimal Height { get => IsValid ? Math.Abs((decimal)_end.Y - (decimal)_start.Y) : 0; set => Resize(value, false); }

    /// <summary>Gets or sets the nine-point resize anchor in row-major order.</summary>
    public int AnchorIndex { get => _anchorIndex; set => this.RaiseAndSetIfChanged(ref _anchorIndex, Math.Clamp(value, 0, 8)); }

    private static bool Valid(double value) => double.IsFinite(value) && Math.Abs(value) <= 1e15;

    private void Translate(decimal dx, decimal dy)
    {
        if (!IsValid || _disposed) return;
        Apply(new Snapshot((double)((decimal)_start.X + dx), (double)((decimal)_start.Y + dy),
                           (double)((decimal)_end.X + dx), (double)((decimal)_end.Y + dy)));
    }

    private void Resize(decimal value, bool widthChanged)
    {
        if (!IsValid || _disposed || value < 0 || value > 1e15m) return;
        decimal width = widthChanged ? value : Width;
        decimal height = widthChanged ? Height : value;
        if (IsAspectLocked && Width > 0 && Height > 0)
        {
            try
            {
                if (widthChanged) height = checked(value * (Height / Width));
                else width = checked(value * (Width / Height));
            }
            catch (OverflowException) { return; }
        }
        if (width > 1e15m || height > 1e15m) return;
        decimal left = X + (Width - width) * (AnchorIndex % 3) / 2;
        decimal top = Y + (Height - height) * (AnchorIndex / 3) / 2;
        Apply(new Snapshot(
            (double)(_start.X <= _end.X ? left : left + width),
            (double)(_start.Y <= _end.Y ? top : top + height),
            (double)(_start.X <= _end.X ? left + width : left),
            (double)(_start.Y <= _end.Y ? top + height : top)));
    }

    private void Apply(Snapshot next)
    {
        if (!Valid(next.StartX) || !Valid(next.StartY) || !Valid(next.EndX) || !Valid(next.EndY)) return;
        var previous = new Snapshot(_start.X, _start.Y, _end.X, _end.Y);
        if (previous == next) return;
        // The history callback owns the original points, not this disposable presentation adapter.
        PointShapeViewModel start = _start, end = _end;
        void Update(Snapshot state)
        {
            start.X = state.StartX;
            start.Y = state.StartY;
            end.X = state.EndX;
            end.Y = state.EndY;
        }
        _history?.Snapshot(previous, next, Update);
        _updating = true;
        try { Update(next); }
        finally { _updating = false; NotifyBounds(); }
    }

    private void OnPointChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!_updating && (e.PropertyName is null or nameof(PointShapeViewModel.X) or nameof(PointShapeViewModel.Y))) NotifyBounds();
    }

    private void NotifyBounds()
    {
        this.RaisePropertyChanged(nameof(IsValid));
        this.RaisePropertyChanged(nameof(X));
        this.RaisePropertyChanged(nameof(Y));
        this.RaisePropertyChanged(nameof(Width));
        this.RaisePropertyChanged(nameof(Height));
    }

    /// <summary>Releases point subscriptions. Already recorded undo/redo operations remain valid.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _start.PropertyChanged -= OnPointChanged;
        if (!ReferenceEquals(_start, _end)) _end.PropertyChanged -= OnPointChanged;
    }

    private readonly record struct Snapshot(double StartX, double StartY, double EndX, double EndY);
}
