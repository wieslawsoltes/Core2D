// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace Core2D.ViewModels.Style;

public partial class FillStyleViewModel : ViewModelBase
{
    [AutoNotify] private BaseColorViewModel? _color;

    public FillStyleViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new FillStyleViewModel(ServiceProvider)
        {
            Name = Name,
            Color = _color?.CopyShared(shared)
        };

        return copy;
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        if (_color != null)
        {
            isDirty |= _color.IsDirty();
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();
        _color?.Invalidate();
    }

    public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
    {
        var mainDisposable = new CompositeDisposable();
        var disposablePropertyChanged = default(IDisposable);
        var disposableColor = default(IDisposable);

        ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
        ObserveObject(_color, ref disposableColor, mainDisposable, observer);

        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Color))
            {
                ObserveObject(_color, ref disposableColor, mainDisposable, observer);
            }

            observer.OnNext((sender, e));
        }

        return mainDisposable;
    }
}
