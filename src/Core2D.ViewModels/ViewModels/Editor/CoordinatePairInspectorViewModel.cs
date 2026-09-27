// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using Core2D.Model.History;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Shapes;
using ReactiveUI;

namespace Core2D.ViewModels.Editor;

/// <summary>Edits an existing point, path size or page size without replacing its model identity.</summary>
public sealed class CoordinatePairInspectorViewModel : ReactiveObject, IDisposable
{
    private const decimal Limit = 1000000000000000m;
    private readonly ViewModelBase _source;
    private readonly IHistory? _history;
    private bool _disposed;
    private bool _aspectLocked;

    /// <summary>Adapts a supported coordinate pair and optional document history.</summary>
    public CoordinatePairInspectorViewModel(ViewModelBase source, IHistory? history)
    {
        if (source is not (PointShapeViewModel or PathSizeViewModel or TemplateContainerViewModel))
            throw new ArgumentException("Expected a point, path size or template size.", nameof(source));
        _source = source;
        _history = history;
        source.PropertyChanged += OnSourceChanged;
        Swap = ReactiveCommand.Create(SwapCore, this.WhenAnyValue(x => x.CanEdit, x => x.IsSize,
            (editable, size) => editable && size), outputScheduler: ImmediateScheduler.Instance);
    }
    /// <summary>Gets whether the pair represents dimensions rather than a position.</summary>
    public bool IsSize => _source is not PointShapeViewModel;
    /// <summary>Gets the first field label.</summary>
    public string FirstLabel => IsSize ? "W" : "X";
    /// <summary>Gets the second field label.</summary>
    public string SecondLabel => IsSize ? "H" : "Y";
    /// <summary>Gets the inclusive lower bound in existing document units.</summary>
    public decimal Minimum => IsSize ? 0m : -Limit;
    /// <summary>Gets the inclusive upper bound in existing document units.</summary>
    public decimal Maximum => Limit;
    /// <summary>Gets whether both source coordinates are finite, bounded and editable.</summary>
    public bool CanEdit
    {
        get
        {
            Pair pair = Read(_source);
            return !_disposed && Valid(pair.First) && Valid(pair.Second)
                && (_source is not PointShapeViewModel point || !point.State.HasFlag(ShapeStateFlags.Locked));
        }
    }
    /// <summary>Gets or sets the first coordinate, committing one history operation.</summary>
    public decimal? First { get => Convert(Read(_source).First); set => SetCoordinate(value, true); }
    /// <summary>Gets or sets the second coordinate, committing one history operation.</summary>
    public decimal? Second { get => Convert(Read(_source).Second); set => SetCoordinate(value, false); }
    /// <summary>Gets or sets proportional dimension editing. Point coordinates are never linked.</summary>
    public bool IsAspectLocked
    {
        get => _aspectLocked;
        set { if (!_disposed) this.RaiseAndSetIfChanged(ref _aspectLocked, value && IsSize); }
    }
    /// <summary>Swaps dimensions in one undoable operation; it never rotates document content.</summary>
    public ReactiveCommand<Unit, Unit> Swap { get; }

    private bool Valid(double value) => double.IsFinite(value) && value >= (double)Minimum && value <= (double)Maximum;
    private static decimal? Convert(double value) => double.IsFinite(value) && Math.Abs(value) <= (double)Limit ? (decimal)value : null;
    private void SetCoordinate(decimal? value, bool first)
    {
        if (!CanEdit || value is not { } number || number < Minimum || number > Maximum) return;
        Pair previous = Read(_source);
        double requested = (double)number;
        Pair next = first ? previous with { First = requested } : previous with { Second = requested };
        if (IsAspectLocked)
        {
            double basis = first ? previous.First : previous.Second;
            // Zero-sized imported geometry remains recoverable without dividing by zero.
            if (basis > 0)
            {
                double scale = requested / basis;
                next = first ? next with { Second = previous.Second * scale } : next with { First = previous.First * scale };
            }
        }
        Commit(previous, next);
    }
    private void SwapCore()
    {
        if (!CanEdit || !IsSize) return;
        Pair previous = Read(_source);
        Commit(previous, new Pair(previous.Second, previous.First));
    }
    private void Commit(Pair previous, Pair next)
    {
        if (previous == next || !Valid(next.First) || !Valid(next.Second)) return;
        ViewModelBase source = _source;
        _history?.Snapshot(previous, next, pair => Write(source, pair));
        Write(source, next);
    }
    private static Pair Read(ViewModelBase source) => source switch
    {
        PointShapeViewModel point => new Pair(point.X, point.Y),
        PathSizeViewModel size => new Pair(size.Width, size.Height),
        TemplateContainerViewModel page => new Pair(page.Width, page.Height),
        _ => default
    };
    private static void Write(ViewModelBase source, Pair pair)
    {
        switch (source)
        {
            case PointShapeViewModel point: point.X = pair.First; point.Y = pair.Second; break;
            case PathSizeViewModel size: size.Width = pair.First; size.Height = pair.Second; break;
            case TemplateContainerViewModel page: page.Width = pair.First; page.Height = pair.Second; break;
        }
    }
    private void OnSourceChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is "X" or "Y" or "Width" or "Height" or "State" or null or "") Refresh();
    }
    private void Refresh()
    {
        this.RaisePropertyChanged(nameof(First));
        this.RaisePropertyChanged(nameof(Second));
        this.RaisePropertyChanged(nameof(CanEdit));
    }
    /// <summary>Releases the model subscription and rejects stale edits; undo retains only the original model.</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _source.PropertyChanged -= OnSourceChanged;
        Refresh();
        Swap.Dispose();
    }
    private readonly record struct Pair(double First, double Second);
}
