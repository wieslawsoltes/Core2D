#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using Core2D.Model.Style;

namespace Core2D.ViewModels.Style
{
    public partial class StrokeStyleViewModel : ViewModelBase
    {
        public static LineCap[] LineCapValues { get; } = (LineCap[])Enum.GetValues(typeof(LineCap));

        public static ArrowType[] ArrowTypeValues { get; } = (ArrowType[])Enum.GetValues(typeof(ArrowType));

        [AutoNotify] private BaseColorViewModel? _color;
        [AutoNotify] private double _thickness;
        [AutoNotify] private LineCap _lineCap;
        [AutoNotify] private string? _dashes;
        [AutoNotify] private double _dashOffset;
        [AutoNotify] private ArrowStyleViewModel? _startArrow;
        [AutoNotify] private ArrowStyleViewModel ?_endArrow;

        public StrokeStyleViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var copy = new StrokeStyleViewModel(ServiceProvider)
            {
                Name = Name,
                Color = _color?.CopyShared(shared),
                Thickness = _thickness,
                LineCap = _lineCap,
                Dashes = _dashes,
                DashOffset = _dashOffset,
                StartArrow = _startArrow?.CopyShared(shared),
                EndArrow = _endArrow?.CopyShared(shared)
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

            if (_startArrow != null)
            {
                isDirty |= _startArrow.IsDirty();
            }

            if (_endArrow != null)
            {
                isDirty |= _endArrow.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            _color?.Invalidate();
            _startArrow?.Invalidate();
            _endArrow?.Invalidate();
        }

        public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposableColor = default(IDisposable);
            var disposableStartArrow = default(IDisposable);
            var disposableEndArrow = default(IDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_color, ref disposableColor, mainDisposable, observer);
            ObserveObject(_startArrow, ref disposableStartArrow, mainDisposable, observer);
            ObserveObject(_endArrow, ref disposableEndArrow, mainDisposable, observer);

            void Handler(object? sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(Color))
                {
                    ObserveObject(_color, ref disposableColor, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(StartArrow))
                {
                    ObserveObject(_startArrow, ref disposableStartArrow, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(EndArrow))
                {
                    ObserveObject(_endArrow, ref disposableEndArrow, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }
    }
}
