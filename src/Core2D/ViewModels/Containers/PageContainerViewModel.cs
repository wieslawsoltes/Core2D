#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.Serialization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Core2D.ViewModels.Editor;

namespace Core2D.ViewModels.Containers;

public partial class PageContainerViewModel : FrameContainerViewModel
{
    [AutoNotify] private TemplateContainerViewModel? _template;

    public PageContainerViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        InsertPageBefore = new RelayCommand<object?>(x => GetProject()?.OnInsertPageBefore(x));
            
        InsertPageAfter = new RelayCommand<object?>(x => GetProject()?.OnInsertPageAfter(x));
            
        ProjectContainerViewModel? GetProject() => ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
    }

    [IgnoreDataMember]
    public ICommand InsertPageBefore { get; }

    [IgnoreDataMember]
    public ICommand InsertPageAfter { get; }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var layers = _layers.CopyShared(shared).ToImmutable();
        var currentLayer = _currentLayer.GetCurrentItem(ref _layers, ref layers);
        var currentShape = _currentShape.GetCurrentItem(ref _layers, ref layers, x => x.Shapes);
        var properties = _properties.CopyShared(shared).ToImmutable();

        var copy = new PageContainerViewModel(ServiceProvider)
        {
            Name = Name,
            IsVisible = IsVisible,
            IsExpanded = IsExpanded,
            Layers = layers,
            CurrentLayer = currentLayer,
            WorkingLayer = _workingLayer?.CopyShared(shared),
            HelperLayer = _helperLayer?.CopyShared(shared),
            CurrentShape = currentShape,
            Properties = properties,
            Record = _record,
            Template = _template?.CopyShared(shared)
        };

        return copy;
    }

    public override void InvalidateLayer()
    {
        base.InvalidateLayer();

        _template?.InvalidateLayer();
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        if (_template is { })
        {
            isDirty |= _template.IsDirty();
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        _template?.Invalidate();
    }

    public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
    {
        var mainDisposable = new CompositeDisposable();
        var disposablePropertyChanged = default(IDisposable);
        var disposableLayers = default(CompositeDisposable);
        var disposableWorkingLayer = default(IDisposable);
        var disposableHelperLayer = default(IDisposable);
        var disposableProperties = default(CompositeDisposable);
        var disposableRecord = default(IDisposable);
        var disposableTemplate = default(IDisposable);

        ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
        ObserveList(_layers, ref disposableLayers, mainDisposable, observer);
        ObserveObject(_workingLayer, ref disposableWorkingLayer, mainDisposable, observer);
        ObserveObject(_helperLayer, ref disposableHelperLayer, mainDisposable, observer);
        ObserveList(_properties, ref disposableProperties, mainDisposable, observer);
        ObserveObject(_record, ref disposableRecord, mainDisposable, observer);
        ObserveObject(_template, ref disposableTemplate, mainDisposable, observer);

        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Layers))
            {
                ObserveList(_layers, ref disposableLayers, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(WorkingLayer))
            {
                ObserveObject(_workingLayer, ref disposableWorkingLayer, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(HelperLayer))
            {
                ObserveObject(_helperLayer, ref disposableHelperLayer, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(Properties))
            {
                ObserveList(_properties, ref disposableProperties, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(Record))
            {
                ObserveObject(_record, ref disposableRecord, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(Template))
            {
                ObserveObject(_template, ref disposableTemplate, mainDisposable, observer);
            }

            observer.OnNext((sender, e));
        }

        return mainDisposable;
    }
}
