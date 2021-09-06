#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Renderer
{
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
        [AutoNotify] private IDecorator? _decorator;

        public ShapeRendererStateViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
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

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _pointStyle?.Invalidate();
            _selectedPointStyle?.Invalidate();
            _selectionStyle?.Invalidate();
            _helperStyle?.Invalidate();
        }

        public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposablePointStyle = default(IDisposable);
            var disposableSelectedPointStyle = default(IDisposable);
            var disposableSelectionStyle = default(IDisposable);
            var disposableHelperStyle = default(IDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_pointStyle, ref disposablePointStyle, mainDisposable, observer);
            ObserveObject(_selectedPointStyle, ref disposableSelectedPointStyle, mainDisposable, observer);
            ObserveObject(_selectionStyle, ref disposableSelectionStyle, mainDisposable, observer);
            ObserveObject(_helperStyle, ref disposableHelperStyle, mainDisposable, observer);

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

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }
    }
}
