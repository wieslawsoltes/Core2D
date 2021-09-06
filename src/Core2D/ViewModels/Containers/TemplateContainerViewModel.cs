#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Containers
{
    public partial class TemplateContainerViewModel : FrameContainerViewModel, IGrid
    {
        [AutoNotify] private double _width;
        [AutoNotify] private double _height;
        [AutoNotify] private BaseColorViewModel? _background;
        [AutoNotify] private bool _isGridEnabled;
        [AutoNotify] private bool _isBorderEnabled;
        [AutoNotify] private double _gridOffsetLeft;
        [AutoNotify] private double _gridOffsetTop;
        [AutoNotify] private double _gridOffsetRight;
        [AutoNotify] private double _gridOffsetBottom;
        [AutoNotify] private double _gridCellWidth;
        [AutoNotify] private double _gridCellHeight;
        [AutoNotify] private BaseColorViewModel? _gridStrokeColor;
        [AutoNotify] private double _gridStrokeThickness;

        public TemplateContainerViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var layers = _layers.Copy(shared).ToImmutable();
            var currentLayerIndex = _currentLayer is null ? -1 : _layers.IndexOf(_currentLayer);
            var currentLayer = currentLayerIndex == -1 ? null : layers[currentLayerIndex];

            var currentShapeIndex = -1;
            var currentShapeLayer = default(LayerContainerViewModel?);
            var currentShape = default(BaseShapeViewModel?);
            if (_currentShape is not null)
            {
                foreach (var layer in _layers)
                {
                    var index = layer.Shapes.IndexOf(_currentShape);
                    if (index >= 0)
                    {
                        currentShapeLayer = layer;
                        currentShapeIndex = index;
                        break;
                    }
                }
            }

            if (_currentShape is not null && currentShapeIndex != -1 && currentShapeLayer is not null)
            {
                var currentShapeLayerIndex = _layers.IndexOf(currentShapeLayer);
                if (currentShapeLayerIndex >= 0)
                {
                    currentShape = layers[currentShapeLayerIndex].Shapes[currentShapeIndex];
                }
            }

            var properties = _properties.Copy(shared).ToImmutable();

            return new TemplateContainerViewModel(ServiceProvider)
            {
                Name = Name,
                IsVisible = IsVisible,
                IsExpanded = IsExpanded,
                Layers = layers,
                CurrentLayer = currentLayer,
                WorkingLayer = (LayerContainerViewModel?)_workingLayer?.Copy(shared),
                HelperLayer = (LayerContainerViewModel?)_helperLayer?.Copy(shared),
                CurrentShape = currentShape,
                Properties = properties,
                Record = _record,
                Width = Width,
                Height = Height,
                Background = (BaseColorViewModel?)_background?.Copy(shared),
                IsGridEnabled = IsGridEnabled,
                IsBorderEnabled = IsBorderEnabled,
                GridOffsetLeft = GridOffsetLeft,
                GridOffsetTop = GridOffsetTop,
                GridOffsetRight = GridOffsetRight,
                GridOffsetBottom = GridOffsetBottom,
                GridCellWidth = GridCellWidth,
                GridCellHeight = GridCellHeight,
                GridStrokeColor = (BaseColorViewModel?)_gridStrokeColor?.Copy(shared),
                GridStrokeThickness = GridStrokeThickness
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (_background is { })
            {
                isDirty |= _background.IsDirty();
            }

            if (_gridStrokeColor is { })
            {
                isDirty |= _gridStrokeColor.IsDirty();
            }

            return isDirty;
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
            var disposableBackground = default(IDisposable);
            var disposableGridStrokeColor = default(IDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveList(_layers, ref disposableLayers, mainDisposable, observer);
            ObserveObject(_workingLayer, ref disposableWorkingLayer, mainDisposable, observer);
            ObserveObject(_helperLayer, ref disposableHelperLayer, mainDisposable, observer);
            ObserveList(_properties, ref disposableProperties, mainDisposable, observer);
            ObserveObject(_record, ref disposableRecord, mainDisposable, observer);
            ObserveObject(_background, ref disposableBackground, mainDisposable, observer);
            ObserveObject(_gridStrokeColor, ref disposableGridStrokeColor, mainDisposable, observer);

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

                if (e.PropertyName == nameof(Background))
                {
                    ObserveObject(_background, ref disposableBackground, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(GridStrokeColor))
                {
                    ObserveObject(_gridStrokeColor, ref disposableGridStrokeColor, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }
    }
}
