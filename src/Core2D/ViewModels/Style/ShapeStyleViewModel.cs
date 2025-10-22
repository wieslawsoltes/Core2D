// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.Serialization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Editor;

namespace Core2D.ViewModels.Style;

public partial class ShapeStyleViewModel : ViewModelBase
{
    [AutoNotify] private StrokeStyleViewModel? _stroke;
    [AutoNotify] private FillStyleViewModel? _fill;
    [AutoNotify] private TextStyleViewModel? _textStyle;

    public ShapeStyleViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        RemoveStyle = new RelayCommand<ShapeStyleViewModel?>(x => GetProject()?.OnRemoveStyle(x));

        ExportStyle = new RelayCommand<ShapeStyleViewModel?>(x => GetProject()?.OnExportStyle(x));

        ProjectContainerViewModel? GetProject() => ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
    }

    [IgnoreDataMember]
    public ICommand RemoveStyle { get; }

        
    [IgnoreDataMember]
    public ICommand ExportStyle { get; }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new ShapeStyleViewModel(ServiceProvider)
        {
            Name = Name,
            Stroke = _stroke?.CopyShared(shared),
            Fill = _fill?.CopyShared(shared),
            TextStyle = _textStyle?.CopyShared(shared)
        };

        return copy;
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        if (_stroke != null)
        {
            isDirty |= _stroke.IsDirty();
        }

        if (_fill != null)
        {
            isDirty |= _fill.IsDirty();
        }

        if (_textStyle != null)
        {
            isDirty |= _textStyle.IsDirty();
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();
        _stroke?.Invalidate();
        _fill?.Invalidate();
        _textStyle?.Invalidate();
    }

    public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
    {
        var mainDisposable = new CompositeDisposable();
        var disposablePropertyChanged = default(IDisposable);
        var disposableStroke = default(IDisposable);
        var disposableFill = default(IDisposable);
        var disposableTextStyle = default(IDisposable);

        ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
        ObserveObject(_stroke, ref disposableStroke, mainDisposable, observer);
        ObserveObject(_fill, ref disposableFill, mainDisposable, observer);
        ObserveObject(_textStyle, ref disposableTextStyle, mainDisposable, observer);

        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Stroke))
            {
                ObserveObject(_stroke, ref disposableStroke, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(Fill))
            {
                ObserveObject(_fill, ref disposableFill, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(TextStyle))
            {
                ObserveObject(_textStyle, ref disposableTextStyle, mainDisposable, observer);
            }

            observer.OnNext((sender, e));
        }

        return mainDisposable;
    }
}
