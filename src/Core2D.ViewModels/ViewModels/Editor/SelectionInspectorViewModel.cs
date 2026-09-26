// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;
using Core2D.Model.History;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using ReactiveUI;

namespace Core2D.ViewModels.Editor;

/// <summary>
/// A document-scoped multi-selection editor. Geometry edits are all-or-nothing and preserve
/// existing point identity. Appearance switches represent mixed values with null.
/// </summary>
public sealed class SelectionInspectorViewModel : ReactiveObject, IDisposable
{
    private const decimal CoordinateLimit = 1e15m;
    private readonly BaseShapeViewModel[] _shapes;
    private readonly IHistory? _history;
    private readonly List<PointShapeViewModel> _points = new();
    private bool _supported;
    private bool _updating;
    private bool _disposed;
    private bool _isAspectLocked;
    private int _anchorIndex;
    private decimal _x, _y, _width, _height;

    /// <summary>Creates a snapshot of the selection, observing subsequent property/point changes.</summary>
    public SelectionInspectorViewModel(IEnumerable<BaseShapeViewModel> shapes, IHistory? history = null)
    {
        ArgumentNullException.ThrowIfNull(shapes);
        var unique = new HashSet<BaseShapeViewModel>(ReferenceEqualityComparer.Instance);
        var ordered = new List<BaseShapeViewModel>();
        foreach (BaseShapeViewModel shape in shapes)
        {
            if (shape is not null && unique.Add(shape)) ordered.Add(shape);
        }
        _shapes = ordered.ToArray();
        _history = history;
        foreach (BaseShapeViewModel shape in _shapes) shape.PropertyChanged += OnShapeChanged;
        RefreshPoints();
        var canTransform = this.WhenAnyValue(x => x.CanEditGeometry);
        FlipHorizontal = ReactiveCommand.Create(() => Flip(true), canTransform);
        FlipVertical = ReactiveCommand.Create(() => Flip(false), canTransform);
    }

    /// <summary>Gets the number of unique selected objects.</summary>
    public int Count => _shapes.Length;
    /// <summary>Gets a compact description for the inspector header.</summary>
    public string Title => $"{Count} objects selected";
    /// <summary>Gets whether all objects and their editable points are unlocked.</summary>
    public bool CanEditAppearance => !_disposed && Count > 0 && !AnyLocked();
    /// <summary>Gets whether the entire selection has supported, finite, independent corner geometry.</summary>
    public bool CanEditGeometry => !_disposed && _supported && CanEditAppearance && PointsAreEditable();
    /// <summary>Gets a visible explanation instead of silently editing only supported objects.</summary>
    public string GeometryMessage => AnyLocked() ? "Unlock the selection to edit its geometry."
        : !_supported ? "Combined resizing supports rectangles, ellipses, text and images. Other geometry remains available in its individual inspector."
        : !PointsAreEditable() ? "Connected, locked or invalid corner points cannot be transformed here." : string.Empty;
    /// <summary>Gets or sets the left edge of the collective bounds.</summary>
    public decimal X { get => _x; set { if (value >= -CoordinateLimit && value <= CoordinateLimit) Translate(value - _x, 0); } }
    /// <summary>Gets or sets the top edge of the collective bounds.</summary>
    public decimal Y { get => _y; set { if (value >= -CoordinateLimit && value <= CoordinateLimit) Translate(0, value - _y); } }
    /// <summary>Gets or sets the collective width, scaling positions and widths about the anchor.</summary>
    public decimal Width { get => _width; set => Resize(value, true); }
    /// <summary>Gets or sets the collective height, scaling positions and heights about the anchor.</summary>
    public decimal Height { get => _height; set => Resize(value, false); }
    /// <summary>Gets or sets whether a non-degenerate selection retains its aspect ratio.</summary>
    public bool IsAspectLocked { get => _isAspectLocked; set => this.RaiseAndSetIfChanged(ref _isAspectLocked, value); }
    /// <summary>Gets or sets the resize origin in row-major 3 by 3 order.</summary>
    public int AnchorIndex { get => _anchorIndex; set => this.RaiseAndSetIfChanged(ref _anchorIndex, Math.Clamp(value, 0, 8)); }
    /// <summary>Gets the common fill visibility, or null for mixed values; sets all selected objects.</summary>
    public bool? IsFilled { get => Common(s => s.IsFilled); set { if (value is { } flag && CanEditAppearance) SetAppearance(true, flag); } }
    /// <summary>Gets the common stroke visibility, or null for mixed values; sets all selected objects.</summary>
    public bool? IsStroked { get => Common(s => s.IsStroked); set { if (value is { } flag && CanEditAppearance) SetAppearance(false, flag); } }
    /// <summary>Gets or sets visibility without discarding other state bits.</summary>
    public bool? IsVisible { get => Common(s => s.State.HasFlag(ShapeStateFlags.Visible)); set { if (value is { } flag) SetState(ShapeStateFlags.Visible, flag); } }
    /// <summary>Gets or sets the lock bit; mixed lock selections can always be unlocked.</summary>
    public bool? IsLocked { get => Common(s => s.State.HasFlag(ShapeStateFlags.Locked)); set { if (value is { } flag) SetState(ShapeStateFlags.Locked, flag); } }
    /// <summary>Reflects all editable corners around the collective horizontal center.</summary>
    public ReactiveCommand<Unit, Unit> FlipHorizontal { get; }
    /// <summary>Reflects all editable corners around the collective vertical center.</summary>
    public ReactiveCommand<Unit, Unit> FlipVertical { get; }

    private bool AnyLocked()
    {
        foreach (BaseShapeViewModel shape in _shapes)
            if (shape.State.HasFlag(ShapeStateFlags.Locked)) return true;
        return false;
    }

    private bool? Common(Func<BaseShapeViewModel, bool> read)
    {
        if (Count == 0) return null;
        bool first = read(_shapes[0]);
        foreach (BaseShapeViewModel shape in _shapes) if (read(shape) != first) return null;
        return first;
    }

    private static bool TryCorners(BaseShapeViewModel shape, out PointShapeViewModel? start, out PointShapeViewModel? end)
    {
        (start, end) = shape switch
        {
            RectangleShapeViewModel rectangle => (rectangle.TopLeft, rectangle.BottomRight),
            EllipseShapeViewModel ellipse => (ellipse.TopLeft, ellipse.BottomRight),
            TextShapeViewModel text => (text.TopLeft, text.BottomRight),
            ImageShapeViewModel image => (image.TopLeft, image.BottomRight),
            _ => (null, null)
        };
        return start is not null && end is not null && !ReferenceEquals(start, end);
    }

    private void RefreshPoints()
    {
        foreach (PointShapeViewModel point in _points) point.PropertyChanged -= OnPointChanged;
        _points.Clear();
        var unique = new HashSet<PointShapeViewModel>(ReferenceEqualityComparer.Instance);
        _supported = Count > 0;
        foreach (BaseShapeViewModel shape in _shapes)
        {
            if (!TryCorners(shape, out var start, out var end)) { _supported = false; continue; }
            if (unique.Add(start!)) _points.Add(start!);
            if (unique.Add(end!)) _points.Add(end!);
        }
        foreach (PointShapeViewModel point in _points) point.PropertyChanged += OnPointChanged;
        Notify();
    }

    private static bool Valid(double value) => double.IsFinite(value) && Math.Abs(value) <= (double)CoordinateLimit;
    private bool PointsAreEditable()
    {
        if (_points.Count == 0) return false;
        foreach (PointShapeViewModel point in _points)
            if (!Valid(point.X) || !Valid(point.Y) || (point.State & (ShapeStateFlags.Locked | ShapeStateFlags.Connector)) != 0) return false;
        return true;
    }

    private void Translate(decimal dx, decimal dy)
    {
        if (!CanEditGeometry) return;
        try { Transform((x, y) => (checked(x + dx), checked(y + dy))); }
        catch (OverflowException) { /* Invalid arithmetic never mutates the document. */ }
    }

    private void Resize(decimal value, bool horizontal)
    {
        if (!CanEditGeometry || value < 0 || value > CoordinateLimit) return;
        decimal old = horizontal ? Width : Height;
        if (old == 0) return; // A collapsed multi-selection has no recoverable relative positions.
        try
        {
            decimal scale = value / old;
            decimal sx = horizontal || IsAspectLocked ? scale : 1;
            decimal sy = !horizontal || IsAspectLocked ? scale : 1;
            decimal pivotX = X + Width * (AnchorIndex % 3) / 2;
            decimal pivotY = Y + Height * (AnchorIndex / 3) / 2;
            Transform((x, y) => (checked(pivotX + (x - pivotX) * sx), checked(pivotY + (y - pivotY) * sy)));
        }
        catch (OverflowException) { }
    }

    private void Flip(bool horizontal)
    {
        if (!CanEditGeometry) return;
        decimal cx = X + Width / 2, cy = Y + Height / 2;
        Transform((x, y) => horizontal ? (2 * cx - x, y) : (x, 2 * cy - y));
    }

    private void Transform(Func<decimal, decimal, (decimal X, decimal Y)> transform)
    {
        var before = new PointState[_points.Count];
        var after = new PointState[_points.Count];
        bool changed = false;
        for (int i = 0; i < _points.Count; i++)
        {
            PointShapeViewModel point = _points[i];
            var next = transform((decimal)point.X, (decimal)point.Y);
            if (Math.Abs(next.X) > CoordinateLimit || Math.Abs(next.Y) > CoordinateLimit) return;
            before[i] = new PointState(point, point.X, point.Y);
            after[i] = new PointState(point, (double)next.X, (double)next.Y);
            changed |= before[i] != after[i];
        }
        if (!changed) return;
        static void Apply(PointState[] states)
        {
            foreach (PointState state in states) { state.Point.X = state.X; state.Point.Y = state.Y; }
        }
        _history?.Snapshot(before, after, Apply);
        _updating = true;
        try { Apply(after); }
        finally { _updating = false; Notify(); }
    }

    private void SetAppearance(bool fill, bool value)
    {
        var before = new AppearanceState[Count];
        var after = new AppearanceState[Count];
        bool changed = false;
        for (int i = 0; i < Count; i++)
        {
            BaseShapeViewModel shape = _shapes[i];
            before[i] = new AppearanceState(shape, shape.IsFilled, shape.IsStroked);
            after[i] = fill ? before[i] with { Fill = value } : before[i] with { Stroke = value };
            changed |= before[i] != after[i];
        }
        if (!changed) return;
        static void Apply(AppearanceState[] states)
        {
            foreach (AppearanceState state in states) { state.Shape.IsFilled = state.Fill; state.Shape.IsStroked = state.Stroke; }
        }
        _history?.Snapshot(before, after, Apply);
        _updating = true;
        try { Apply(after); }
        finally { _updating = false; Notify(); }
    }

    private void SetState(ShapeStateFlags mask, bool value)
    {
        if (_disposed || Count == 0) return;
        var before = new FlagState[Count];
        var after = new FlagState[Count];
        bool changed = false;
        for (int i = 0; i < Count; i++)
        {
            BaseShapeViewModel shape = _shapes[i];
            before[i] = new FlagState(shape, shape.State);
            after[i] = new FlagState(shape, value ? shape.State | mask : shape.State & ~mask);
            changed |= before[i] != after[i];
        }
        if (!changed) return;
        static void Apply(FlagState[] states)
        {
            foreach (FlagState state in states) state.Shape.State = state.Flags;
        }
        _history?.Snapshot(before, after, Apply);
        _updating = true;
        try { Apply(after); }
        finally { _updating = false; Notify(); }
    }

    private void OnShapeChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_updating || _disposed) return;
        if (e.PropertyName is null or "TopLeft" or "BottomRight") RefreshPoints();
        else if (e.PropertyName is nameof(BaseShapeViewModel.State) or nameof(BaseShapeViewModel.IsFilled) or nameof(BaseShapeViewModel.IsStroked)) Notify();
    }

    private void OnPointChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!_updating && !_disposed && (e.PropertyName is null or nameof(PointShapeViewModel.X) or nameof(PointShapeViewModel.Y) or nameof(PointShapeViewModel.State))) Notify();
    }

    private void Notify()
    {
        _x = _y = _width = _height = 0;
        if (_supported && PointsAreEditable())
        {
            decimal left = decimal.MaxValue, top = decimal.MaxValue, right = decimal.MinValue, bottom = decimal.MinValue;
            foreach (PointShapeViewModel point in _points)
            {
                left = Math.Min(left, (decimal)point.X); top = Math.Min(top, (decimal)point.Y);
                right = Math.Max(right, (decimal)point.X); bottom = Math.Max(bottom, (decimal)point.Y);
            }
            _x = left; _y = top; _width = right - left; _height = bottom - top;
        }
        this.RaisePropertyChanged(nameof(CanEditAppearance)); this.RaisePropertyChanged(nameof(CanEditGeometry));
        this.RaisePropertyChanged(nameof(GeometryMessage));
        this.RaisePropertyChanged(nameof(X)); this.RaisePropertyChanged(nameof(Y));
        this.RaisePropertyChanged(nameof(Width)); this.RaisePropertyChanged(nameof(Height));
        this.RaisePropertyChanged(nameof(IsFilled)); this.RaisePropertyChanged(nameof(IsStroked));
        this.RaisePropertyChanged(nameof(IsVisible)); this.RaisePropertyChanged(nameof(IsLocked));
    }

    /// <summary>Releases subscriptions and commands; previously recorded history remains usable.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        foreach (BaseShapeViewModel shape in _shapes) shape.PropertyChanged -= OnShapeChanged;
        foreach (PointShapeViewModel point in _points) point.PropertyChanged -= OnPointChanged;
        _points.Clear();
        FlipHorizontal.Dispose(); FlipVertical.Dispose();
    }

    private readonly record struct PointState(PointShapeViewModel Point, double X, double Y);
    private readonly record struct AppearanceState(BaseShapeViewModel Shape, bool Fill, bool Stroke);
    private readonly record struct FlagState(BaseShapeViewModel Shape, ShapeStateFlags Flags);
}
