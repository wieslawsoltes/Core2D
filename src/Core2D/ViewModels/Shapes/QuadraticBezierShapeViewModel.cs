﻿#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Data;

namespace Core2D.ViewModels.Shapes;

public partial class QuadraticBezierShapeViewModel : BaseShapeViewModel
{
    [AutoNotify] private PointShapeViewModel? _point1;
    [AutoNotify] private PointShapeViewModel? _point2;
    [AutoNotify] private PointShapeViewModel? _point3;

    public QuadraticBezierShapeViewModel(IServiceProvider? serviceProvider) : base(serviceProvider, typeof(QuadraticBezierShapeViewModel))
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new QuadraticBezierShapeViewModel(ServiceProvider)
        {
            Name = Name,
            State = State,
            Style = _style?.CopyShared(shared),
            IsStroked = IsStroked,
            IsFilled = IsFilled,
            Properties = _properties.CopyShared(shared).ToImmutable(),
            Record = _record,
            Point1 = _point1?.CopyShared(shared),
            Point2 = _point2?.CopyShared(shared),
            Point3 = _point3?.CopyShared(shared)
        };

        return copy;
    }

    public override void DrawShape(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        if (State.HasFlag(ShapeStateFlags.Visible))
        {
            renderer?.DrawQuadraticBezier(dc, this, Style);
        }
    }

    public override void DrawPoints(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        if (_point1 is null || _point2 is null || _point3 is null)
        {
            return;
        }

        if (selection?.SelectedShapes is null)
        {
            return;
        }

        if (selection.SelectedShapes.Contains(this))
        {
            _point1.DrawShape(dc, renderer, selection);
            _point2.DrawShape(dc, renderer, selection);
            _point3.DrawShape(dc, renderer, selection);
        }
        else
        {
            if (selection.SelectedShapes.Contains(_point1))
            {
                _point1.DrawShape(dc, renderer, selection);
            }

            if (selection.SelectedShapes.Contains(_point2))
            {
                _point2.DrawShape(dc, renderer, selection);
            }

            if (selection.SelectedShapes.Contains(_point3))
            {
                _point3.DrawShape(dc, renderer, selection);
            }
        }
    }

    public override void Bind(DataFlow dataFlow, object? db, object? r)
    {
        var record = Record ?? r;

        dataFlow.Bind(this, db, record);

        _point1?.Bind(dataFlow, db, record);
        _point2?.Bind(dataFlow, db, record);
        _point3?.Bind(dataFlow, db, record);
    }

    public override void Move(ISelection? selection, decimal dx, decimal dy)
    {
        if (_point1 is null || _point2 is null || _point3 is null)
        {
            return;
        }

        if (!_point1.State.HasFlag(ShapeStateFlags.Connector))
        {
            _point1.Move(selection, dx, dy);
        }

        if (!_point2.State.HasFlag(ShapeStateFlags.Connector))
        {
            _point2.Move(selection, dx, dy);
        }

        if (!_point3.State.HasFlag(ShapeStateFlags.Connector))
        {
            _point3.Move(selection, dx, dy);
        }
    }

    public override void Select(ISelection? selection)
    {
        base.Select(selection);

        _point1?.Select(selection);
        _point2?.Select(selection);
        _point3?.Select(selection);
    }

    public override void Deselect(ISelection? selection)
    {
        base.Deselect(selection);

        _point1?.Deselect(selection);
        _point2?.Deselect(selection);
        _point3?.Deselect(selection);
    }

    public override void GetPoints(IList<PointShapeViewModel> points)
    {
        if (_point1 is null || _point2 is null || _point3 is null)
        {
            return;
        }

        points.Add(_point1);
        points.Add(_point2);
        points.Add(_point3);
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        if (_point1 != null)
        {
            isDirty |= _point1.IsDirty();
        }

        if (_point2 != null)
        {
            isDirty |= _point2.IsDirty();
        }

        if (_point3 != null)
        {
            isDirty |= _point3.IsDirty();
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        _point1?.Invalidate();
        _point2?.Invalidate();
        _point3?.Invalidate();
    }

    public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
    {
        var mainDisposable = new CompositeDisposable();
        var disposablePropertyChanged = default(IDisposable);
        var disposableStyle = default(IDisposable);
        var disposableProperties = default(CompositeDisposable);
        var disposableRecord = default(IDisposable);
        var disposablePoint1 = default(IDisposable);
        var disposablePoint2 = default(IDisposable);
        var disposablePoint3 = default(IDisposable);

        ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
        ObserveObject(_style, ref disposableStyle, mainDisposable, observer);
        ObserveList(_properties, ref disposableProperties, mainDisposable, observer);
        ObserveObject(_record, ref disposableRecord, mainDisposable, observer);
        ObserveObject(_point1, ref disposablePoint1, mainDisposable, observer);
        ObserveObject(_point2, ref disposablePoint2, mainDisposable, observer);
        ObserveObject(_point3, ref disposablePoint3, mainDisposable, observer);

        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Style))
            {
                ObserveObject(_style, ref disposableStyle, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(Properties))
            {
                ObserveList(_properties, ref disposableProperties, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(Record))
            {
                ObserveObject(_record, ref disposableRecord, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(Point1))
            {
                ObserveObject(_point1, ref disposablePoint1, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(Point2))
            {
                ObserveObject(_point2, ref disposablePoint2, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(Point3))
            {
                ObserveObject(_point3, ref disposablePoint3, mainDisposable, observer);
            }

            observer.OnNext((sender, e));
        }

        return mainDisposable;
    }
}