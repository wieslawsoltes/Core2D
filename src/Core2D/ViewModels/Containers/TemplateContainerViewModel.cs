#nullable disable
using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Containers
{
    public partial class TemplateContainerViewModel : FrameContainerViewModel, IGrid
    {
        [AutoNotify] private double _width;
        [AutoNotify] private double _height;
        [AutoNotify] private BaseColorViewModel _background;
        [AutoNotify] private bool _isGridEnabled;
        [AutoNotify] private bool _isBorderEnabled;
        [AutoNotify] private double _gridOffsetLeft;
        [AutoNotify] private double _gridOffsetTop;
        [AutoNotify] private double _gridOffsetRight;
        [AutoNotify] private double _gridOffsetBottom;
        [AutoNotify] private double _gridCellWidth;
        [AutoNotify] private double _gridCellHeight;
        [AutoNotify] private BaseColorViewModel _gridStrokeColor;
        [AutoNotify] private double _gridStrokeThickness;

        public TemplateContainerViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
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
        
        public override IDisposable Subscribe(IObserver<(object sender, PropertyChangedEventArgs e)> observer)
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

            void Handler(object sender, PropertyChangedEventArgs e) 
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
