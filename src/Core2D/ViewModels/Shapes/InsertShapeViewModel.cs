// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor;

namespace Core2D.ViewModels.Shapes;

/// <summary>
/// Insert shape that references a Block and renders its contents using a translation.
/// Creates and maintains virtual connectors based on the referenced Block connectors.
/// </summary>
public partial class InsertShapeViewModel : ConnectableShapeViewModel
{
    [AutoNotify] private BlockShapeViewModel? _block;
    [AutoNotify] private PointShapeViewModel? _point;
    // Persistent proxies used only for computing bounding box during selection/move.
    private ImmutableArray<PointShapeViewModel> _bboxProxies;

    public InsertShapeViewModel(IServiceProvider? serviceProvider) : base(serviceProvider, typeof(InsertShapeViewModel))
    {
        // Ensure non-default arrays to avoid ImmutableArray<T> default instance issues
        Connectors = ImmutableArray.Create<PointShapeViewModel>();
        _bboxProxies = ImmutableArray.Create<PointShapeViewModel>();
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new InsertShapeViewModel(ServiceProvider)
        {
            Name = Name,
            State = State,
            Style = _style?.CopyShared(shared),
            IsStroked = IsStroked,
            IsFilled = IsFilled,
            Properties = _properties.CopyShared(shared).ToImmutable(),
            Record = _record,
            Block = _block, // keep reference to the same block
            Point = (PointShapeViewModel?)_point?.CopyShared(shared)
        };

        if (copy._point is { })
        {
            copy._point.Owner = copy;
        }

        // Initialize connectors snapshot for the copy
        copy.UpdateConnectorsFromBlock();

        return copy;
    }

    public override void DrawShape(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        if (!State.HasFlag(ShapeStateFlags.Visible))
        {
            return;
        }

        var block = _block;
        if (block is null)
        {
            return;
        }

        var dx = _point?.X ?? 0.0;
        var dy = _point?.Y ?? 0.0;

        foreach (var shape in block.Shapes)
        {
            // Render translated clones to avoid mutating original block content
            if (shape is null)
            {
                continue;
            }

            var clone = (BaseShapeViewModel)shape.Copy(null);
            clone.Move(null, (decimal)dx, (decimal)dy);
            clone.DrawShape(dc, renderer, selection);
        }

        base.DrawShape(dc, renderer, selection);
    }

    public override void DrawPoints(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        base.DrawPoints(dc, renderer, selection);
    }

    public override void Bind(DataFlow dataFlow, object? db, object? r)
    {
        var record = Record ?? r;

        foreach (var connector in _connectors)
        {
            connector.Bind(dataFlow, db, record);
        }

        base.Bind(dataFlow, db, record);
    }

    public override void Move(ISelection? selection, decimal dx, decimal dy)
    {
        if (_point is { })
        {
            _point.Move(selection, dx, dy);
            UpdateConnectorsFromBlock();
        }
    }

    public IEnumerable<BlockShapeViewModel> AvailableBlocks
    {
        get
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            var items = project?.CurrentGroupLibrary?.Items;
            if (items is { })
            {
                foreach (var item in items)
                {
                    if (item is BlockShapeViewModel b)
                    {
                        yield return b;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Rebuild or update virtual connectors based on the referenced block connectors.
    /// Smartly preserves existing connector instances (by name or index) and remaps
    /// any external line endpoints that referenced replaced connector instances.
    /// </summary>
    public void UpdateConnectorsFromBlock()
    {
        var dx = _point?.X ?? 0.0;
        var dy = _point?.Y ?? 0.0;

        // Snapshot current connectors (potentially referenced by line endpoints)
        var oldConnectors = !_connectors.IsDefault ? _connectors : ImmutableArray<PointShapeViewModel>.Empty;
        var dst = !_connectors.IsDefault ? _connectors.ToList() : new List<PointShapeViewModel>();
        var srcArr = _block is { } && !_block.Connectors.IsDefault
            ? _block.Connectors
            : ImmutableArray<PointShapeViewModel>.Empty;
        var src = srcArr.ToList();

        // First, try to align by name for stability when names are present
        var srcByName = src.Where(c => !string.IsNullOrWhiteSpace(c.Name)).ToDictionary(c => c.Name);
        var dstByName = dst.Where(c => !string.IsNullOrWhiteSpace(c.Name)).ToDictionary(c => c.Name);
        var oldByName = oldConnectors.Where(c => !string.IsNullOrWhiteSpace(c.Name)).ToDictionary(c => c.Name);

        var usedDst = new HashSet<PointShapeViewModel>();
        var result = new List<PointShapeViewModel>(src.Count);
        var remap = new Dictionary<PointShapeViewModel, PointShapeViewModel>(); // old -> new

        for (int i = 0; i < src.Count; i++)
        {
            var bc = src[i];
            PointShapeViewModel? vc = null;
            if (!string.IsNullOrEmpty(bc.Name) && dstByName.TryGetValue(bc.Name, out var named))
            {
                vc = named;
            }
            else if (i < dst.Count)
            {
                // Fallback: reuse by index if available and not already consumed
                var candidate = dst[i];
                if (!usedDst.Contains(candidate))
                {
                    vc = candidate;
                }
            }

            if (vc is { })
            {
                usedDst.Add(vc);
                vc.X = bc.X + dx;
                vc.Y = bc.Y + dy;
                vc.State |= ShapeStateFlags.Connector;
                vc.State |= ShapeStateFlags.Visible | ShapeStateFlags.Printable;
                vc.State &= ~ShapeStateFlags.Standalone;
                vc.State &= ~(ShapeStateFlags.Input | ShapeStateFlags.Output | ShapeStateFlags.None);
                if (bc.State.HasFlag(ShapeStateFlags.Input)) vc.State |= ShapeStateFlags.Input;
                if (bc.State.HasFlag(ShapeStateFlags.Output)) vc.State |= ShapeStateFlags.Output;
                if (bc.State.HasFlag(ShapeStateFlags.None)) vc.State |= ShapeStateFlags.None;
                // Make insert connectors non-movable; they are derived from the referenced block
                vc.State |= ShapeStateFlags.Locked;
                if (!ReferenceEquals(vc.Owner, this)) vc.Owner = this;
                result.Add(vc);

                // If there was a connector with the same name/index in the old list, record remap for stability
                if (!string.IsNullOrWhiteSpace(bc.Name) && oldByName.TryGetValue(bc.Name, out var oldByNameMatch))
                {
                    remap[oldByNameMatch] = vc;
                }
                else if (i < oldConnectors.Length)
                {
                    remap[oldConnectors[i]] = vc;
                }
            }
            else
            {
                var clone = new PointShapeViewModel(ServiceProvider)
                {
                    Name = bc.Name,
                    Properties = ImmutableArray.Create<PropertyViewModel>(),
                    X = bc.X + dx,
                    Y = bc.Y + dy
                };
                clone.Owner = this;
                clone.State |= ShapeStateFlags.Connector;
                clone.State |= ShapeStateFlags.Visible | ShapeStateFlags.Printable;
                clone.State &= ~ShapeStateFlags.Standalone;
                clone.State &= ~(ShapeStateFlags.Input | ShapeStateFlags.Output | ShapeStateFlags.None);
                if (bc.State.HasFlag(ShapeStateFlags.Input)) clone.State |= ShapeStateFlags.Input;
                if (bc.State.HasFlag(ShapeStateFlags.Output)) clone.State |= ShapeStateFlags.Output;
                if (bc.State.HasFlag(ShapeStateFlags.None)) clone.State |= ShapeStateFlags.None;
                // Make insert connectors non-movable
                clone.State |= ShapeStateFlags.Locked;
                result.Add(clone);

                 // Map old-by-name or old-by-index to the newly created instance
                 if (!string.IsNullOrWhiteSpace(bc.Name) && oldByName.TryGetValue(bc.Name, out var oldByNameMatch2))
                 {
                     remap[oldByNameMatch2] = clone;
                 }
                 else if (i < oldConnectors.Length)
                 {
                     remap[oldConnectors[i]] = clone;
                 }
            }
        }

        // Apply the new connectors list
        var newConnectors = result.ToImmutableArray();
        Connectors = newConnectors;

        // Update bounding box proxy points to match the current block geometry + insertion offset
        UpdateBboxProxies(dx, dy);

        // Remap existing line endpoints that referenced old connector instances to the new ones
        if (!oldConnectors.IsDefaultOrEmpty && ServiceProvider.GetService<ProjectEditorViewModel>()?.Project is { } project)
        {
            var lines = project.GetAllShapes<LineShapeViewModel>().ToList();
            foreach (var kv in remap)
            {
                var oldPt = kv.Key;
                var newPt = kv.Value;

                if (ReferenceEquals(oldPt, newPt))
                {
                    continue; // unchanged
                }

                foreach (var line in lines)
                {
                    if (ReferenceEquals(line.Start, oldPt))
                    {
                        var prev = line.Start;
                        var next = newPt;
                        project.History?.Snapshot(prev, next, p => line.Start = p);
                        line.Start = next;
                    }

                    if (ReferenceEquals(line.End, oldPt))
                    {
                        var prev = line.End;
                        var next = newPt;
                        project.History?.Snapshot(prev, next, p => line.End = p);
                        line.End = next;
                    }
                }
            }
        }
    }

    private void UpdateBboxProxies(double dx, double dy)
    {
        if (_block is null)
        {
            _bboxProxies = ImmutableArray<PointShapeViewModel>.Empty;
            return;
        }

        // Gather all points from referenced block shapes in a stable order
        var sourcePoints = new List<PointShapeViewModel>();
        foreach (var s in _block.Shapes)
        {
            s.GetPoints(sourcePoints);
        }

        var count = sourcePoints.Count;
        if (_bboxProxies.IsDefaultOrEmpty || _bboxProxies.Length != count)
        {
            var builder = ImmutableArray.CreateBuilder<PointShapeViewModel>(count);
            foreach (var p in sourcePoints)
            {
                var proxy = new PointShapeViewModel(ServiceProvider)
                {
                    Properties = ImmutableArray.Create<PropertyViewModel>(),
                    X = p.X + dx,
                    Y = p.Y + dy
                };
                proxy.Owner = this;
                proxy.State |= ShapeStateFlags.Locked;
                builder.Add(proxy);
            }
            _bboxProxies = builder.ToImmutable();
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                var src = sourcePoints[i];
                var proxy = _bboxProxies[i];
                proxy.X = src.X + dx;
                proxy.Y = src.Y + dy;
            }
        }
    }

    public override void GetPoints(IList<PointShapeViewModel> points)
    {
        // Expose the insert anchor point for movement
        if (_point is { })
        {
            points.Add(_point);
        }

        // Contribute persistent translated geometry proxy points to compute bounding box live during move
        if (!_bboxProxies.IsDefaultOrEmpty)
        {
            foreach (var proxy in _bboxProxies)
            {
                points.Add(proxy);
            }
        }

        // Include virtual connectors so decorator shows connector-driven extents too
        if (!_connectors.IsDefault)
        {
            foreach (var c in _connectors)
            {
                points.Add(c);
            }
        }
    }

    public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
    {
        var mainDisposable = new CompositeDisposable();
        var disposablePropertyChanged = default(IDisposable);
        var disposableStyle = default(IDisposable);
        var disposableProperties = default(CompositeDisposable);
        var disposableRecord = default(IDisposable);
        var disposablePoint = default(IDisposable);
        var disposableBlock = default(IDisposable);
        var disposableBlockShapes = default(CompositeDisposable);
        var disposableBlockConnectors = default(CompositeDisposable);
        var disposableConnectors = default(CompositeDisposable);

        // Helper observer that updates connectors and forwards events
        var forwarder = new ForwardObserver(this, observer);

        ObserveSelf(SelfHandler, ref disposablePropertyChanged, mainDisposable);
        ObserveObject(_style, ref disposableStyle, mainDisposable, observer);
        ObserveList(_properties, ref disposableProperties, mainDisposable, observer);
        ObserveObject(_record, ref disposableRecord, mainDisposable, observer);
        ObserveObject(_point, ref disposablePoint, mainDisposable, forwarder);
        ObserveObject(_block, ref disposableBlock, mainDisposable, forwarder);

        if (_block is { })
        {
            ObserveList(_block.Shapes, ref disposableBlockShapes, mainDisposable, forwarder);
            ObserveList(_block.Connectors, ref disposableBlockConnectors, mainDisposable, forwarder);
        }

        ObserveList(_connectors, ref disposableConnectors, mainDisposable, observer);

        return mainDisposable;

        void SelfHandler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Point))
            {
                ObserveObject(_point, ref disposablePoint, mainDisposable, forwarder);
                UpdateConnectorsFromBlock();
            }

            if (e.PropertyName == nameof(Block))
            {
                ObserveObject(_block, ref disposableBlock, mainDisposable, forwarder);
                if (_block is { })
                {
                    ObserveList(_block.Shapes, ref disposableBlockShapes, mainDisposable, forwarder);
                    ObserveList(_block.Connectors, ref disposableBlockConnectors, mainDisposable, forwarder);
                }
                UpdateConnectorsFromBlock();
            }

            observer.OnNext((sender, e));
        }
    }

    private sealed class ForwardObserver : IObserver<(object? sender, PropertyChangedEventArgs e)>
    {
        private readonly InsertShapeViewModel _owner;
        private readonly IObserver<(object? sender, PropertyChangedEventArgs e)> _target;

        public ForwardObserver(InsertShapeViewModel owner, IObserver<(object? sender, PropertyChangedEventArgs e)> target)
        {
            _owner = owner;
            _target = target;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext((object? sender, PropertyChangedEventArgs e) value)
        {
            // Any relevant change in the referenced block, its connectors or the insert point
            // should update virtual connectors
            _owner.UpdateConnectorsFromBlock();
            _target.OnNext(value);
        }
    }
}
