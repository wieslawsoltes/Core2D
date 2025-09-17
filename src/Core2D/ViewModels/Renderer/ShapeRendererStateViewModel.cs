#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Renderer;

public partial class ShapeRendererStateViewModel : ViewModelBase
{
    [AutoNotify] private double _panX;
    [AutoNotify] private double _panY;
    [AutoNotify] private double _zoomX;
    [AutoNotify] private double _zoomY;
    [AutoNotify] private ShapeStateFlags _drawShapeState;
    [AutoNotify] private IImageCache? _imageCache;
    [AutoNotify] private bool _drawDecorators;
    [AutoNotify] private bool _drawPoints;
    [AutoNotify] private ShapeStyleViewModel? _pointStyle;
    [AutoNotify] private ShapeStyleViewModel? _selectedPointStyle;
    [AutoNotify] private double _pointSize;
    [AutoNotify] private ShapeStyleViewModel? _selectionStyle;
    [AutoNotify] private ShapeStyleViewModel? _helperStyle;
    [AutoNotify] private ShapeStyleViewModel? _connectorNoneStyle;
    [AutoNotify] private ShapeStyleViewModel? _connectorInputStyle;
    [AutoNotify] private ShapeStyleViewModel? _connectorOutputStyle;
    [AutoNotify] private ShapeStyleViewModel? _connectorHoverStyle;
    [AutoNotify] private ShapeStyleViewModel? _connectorSelectedStyle;
    [AutoNotify] private IDecorator? _decorator;
    [AutoNotify] private ImmutableHashSet<PointShapeViewModel> _activeConnectionPoints = ImmutableHashSet<PointShapeViewModel>.Empty;

    public ShapeRendererStateViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        if (_pointStyle != null)
        {
            isDirty |= _pointStyle.IsDirty();
        }

        if (_selectedPointStyle != null)
        {
            isDirty |= _selectedPointStyle.IsDirty();
        }

        if (_selectionStyle != null)
        {
            isDirty |= _selectionStyle.IsDirty();
        }

        if (_helperStyle != null)
        {
            isDirty |= _helperStyle.IsDirty();
        }

        if (_connectorNoneStyle != null)
        {
            isDirty |= _connectorNoneStyle.IsDirty();
        }

        if (_connectorInputStyle != null)
        {
            isDirty |= _connectorInputStyle.IsDirty();
        }

        if (_connectorOutputStyle != null)
        {
            isDirty |= _connectorOutputStyle.IsDirty();
        }

        if (_connectorHoverStyle != null)
        {
            isDirty |= _connectorHoverStyle.IsDirty();
        }

        if (_connectorSelectedStyle != null)
        {
            isDirty |= _connectorSelectedStyle.IsDirty();
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        _pointStyle?.Invalidate();
        _selectedPointStyle?.Invalidate();
        _selectionStyle?.Invalidate();
        _helperStyle?.Invalidate();
        _connectorNoneStyle?.Invalidate();
        _connectorInputStyle?.Invalidate();
        _connectorOutputStyle?.Invalidate();
        _connectorHoverStyle?.Invalidate();
        _connectorSelectedStyle?.Invalidate();
    }

    public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
    {
        var mainDisposable = new CompositeDisposable();
        var disposablePropertyChanged = default(IDisposable);
        var disposablePointStyle = default(IDisposable);
        var disposableSelectedPointStyle = default(IDisposable);
        var disposableSelectionStyle = default(IDisposable);
        var disposableHelperStyle = default(IDisposable);
        var disposableConnectorNoneStyle = default(IDisposable);
        var disposableConnectorInputStyle = default(IDisposable);
        var disposableConnectorOutputStyle = default(IDisposable);
        var disposableConnectorHoverStyle = default(IDisposable);
        var disposableConnectorSelectedStyle = default(IDisposable);

        ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
        ObserveObject(_pointStyle, ref disposablePointStyle, mainDisposable, observer);
        ObserveObject(_selectedPointStyle, ref disposableSelectedPointStyle, mainDisposable, observer);
        ObserveObject(_selectionStyle, ref disposableSelectionStyle, mainDisposable, observer);
        ObserveObject(_helperStyle, ref disposableHelperStyle, mainDisposable, observer);
        ObserveObject(_connectorNoneStyle, ref disposableConnectorNoneStyle, mainDisposable, observer);
        ObserveObject(_connectorInputStyle, ref disposableConnectorInputStyle, mainDisposable, observer);
        ObserveObject(_connectorOutputStyle, ref disposableConnectorOutputStyle, mainDisposable, observer);
        ObserveObject(_connectorHoverStyle, ref disposableConnectorHoverStyle, mainDisposable, observer);
        ObserveObject(_connectorSelectedStyle, ref disposableConnectorSelectedStyle, mainDisposable, observer);

        void Handler(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PointStyle))
            {
                ObserveObject(_pointStyle, ref disposablePointStyle, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(SelectedPointStyle))
            {
                ObserveObject(_selectedPointStyle, ref disposableSelectedPointStyle, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(SelectionStyle))
            {
                ObserveObject(_selectionStyle, ref disposableSelectionStyle, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(HelperStyle))
            {
                ObserveObject(_helperStyle, ref disposableHelperStyle, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(ConnectorNoneStyle))
            {
                ObserveObject(_connectorNoneStyle, ref disposableConnectorNoneStyle, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(ConnectorInputStyle))
            {
                ObserveObject(_connectorInputStyle, ref disposableConnectorInputStyle, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(ConnectorOutputStyle))
            {
                ObserveObject(_connectorOutputStyle, ref disposableConnectorOutputStyle, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(ConnectorHoverStyle))
            {
                ObserveObject(_connectorHoverStyle, ref disposableConnectorHoverStyle, mainDisposable, observer);
            }

            if (e.PropertyName == nameof(ConnectorSelectedStyle))
            {
                ObserveObject(_connectorSelectedStyle, ref disposableConnectorSelectedStyle, mainDisposable, observer);
            }

            observer.OnNext((sender, e));
        }

        return mainDisposable;
    }
}
