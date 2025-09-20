// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.Serialization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;

namespace Core2D.ViewModels.Shapes;

public partial class GroupShapeViewModel : ConnectableShapeViewModel
{
    [AutoNotify] private ImmutableArray<BaseShapeViewModel> _shapes;

    public GroupShapeViewModel(IServiceProvider? serviceProvider) : base(serviceProvider, typeof(GroupShapeViewModel))
    {
        EditGroup = new RelayCommand<GroupShapeViewModel?>(x => GetProject()?.OnEditGroup(x));

        ProjectContainerViewModel? GetProject() => ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
    }

    [IgnoreDataMember]
    public ICommand EditGroup { get; }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new GroupShapeViewModel(ServiceProvider)
        {
            Name = Name,
            State = State,
            Style = _style?.CopyShared(shared),
            IsStroked = IsStroked,
            IsFilled = IsFilled,
            Properties = _properties.CopyShared(shared).ToImmutable(),
            Record = _record,
            Connectors = _connectors.CopyShared(shared).ToImmutable(),
            Shapes = _shapes.CopyShared(shared).ToImmutable()
        };

        return copy;
    }

    public override void DrawShape(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        if (State.HasFlag(ShapeStateFlags.Visible))
        {
            foreach (var shape in _shapes)
            {
                shape.DrawShape(dc, renderer, selection);
            }
        }

        base.DrawShape(dc, renderer, selection);
    }

    public override void DrawPoints(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        if (State.HasFlag(ShapeStateFlags.Visible))
        {
            foreach (var shape in _shapes)
            {
                shape.DrawPoints(dc, renderer, selection);
            }
        }

        base.DrawPoints(dc, renderer, selection);
    }

    public override void Bind(DataFlow dataFlow, object? db, object? r)
    {
        var record = Record ?? r;

        foreach (var shape in _shapes)
        {
            shape.Bind(dataFlow, db, record);
        }

        base.Bind(dataFlow, db, record);
    }

    public override void Move(ISelection? selection, decimal dx, decimal dy)
    {
        foreach (var shape in _shapes)
        {
            if (!shape.State.HasFlag(ShapeStateFlags.Connector))
            {
                shape.Move(selection, dx, dy);
            }
        }

        base.Move(selection, dx, dy);
    }

    public override void GetPoints(IList<PointShapeViewModel> points)
    {
        foreach (var shape in _shapes)
        {
            shape.GetPoints(points);
        }

        base.GetPoints(points);
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        foreach (var shape in _shapes)
        {
            isDirty |= shape.IsDirty();
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        foreach (var shape in _shapes)
        {
            shape.Invalidate();
        }
    }

    public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
    {
        var mainDisposable = new CompositeDisposable();
        var disposablePropertyChanged = default(IDisposable);
        var disposableStyle = default(IDisposable);
        var disposableProperties = default(CompositeDisposable);
        var disposableRecord = default(IDisposable);
        var disposableShapes = default(CompositeDisposable);
        var disposableConnectors = default(CompositeDisposable);

        ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
        ObserveObject(_style, ref disposableStyle, mainDisposable, observer);
        ObserveList(_properties, ref disposableProperties, mainDisposable, observer);
        ObserveObject(_record, ref disposableRecord, mainDisposable, observer);
        ObserveList(_shapes, ref disposableShapes, mainDisposable, observer);
        ObserveList(_connectors, ref disposableConnectors, mainDisposable, observer);

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

            if (e.PropertyName == nameof(Shapes))
            {
                ObserveList(_shapes, ref disposableShapes, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(Connectors))
            {
                ObserveList(_connectors, ref disposableConnectors, mainDisposable, observer);
            }

            observer.OnNext((sender, e));
        }

        return mainDisposable;
    }
}
