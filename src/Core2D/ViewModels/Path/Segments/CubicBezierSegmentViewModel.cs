#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path.Segments
{
    public partial class CubicBezierSegmentViewModel : PathSegmentViewModel
    {
        [AutoNotify] private PointShapeViewModel? _point1;
        [AutoNotify] private PointShapeViewModel? _point2;
        [AutoNotify] private PointShapeViewModel? _point3;

        public CubicBezierSegmentViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var copy = new CubicBezierSegmentViewModel(ServiceProvider)
            {
                Name = Name,
                IsStroked = IsStroked,
                Point1 = _point1?.CopyShared(shared),
                Point2 = _point2?.CopyShared(shared),
                Point3 = _point3?.CopyShared(shared),
            };

            return copy;
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
            var disposablePoint1 = default(IDisposable);
            var disposablePoint2 = default(IDisposable);
            var disposablePoint3 = default(IDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_point1, ref disposablePoint1, mainDisposable, observer);
            ObserveObject(_point2, ref disposablePoint2, mainDisposable, observer);
            ObserveObject(_point3, ref disposablePoint3, mainDisposable, observer);

            void Handler(object? sender, PropertyChangedEventArgs e)
            {
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

        public override string ToXamlString()
        {
            if (_point1 is null || _point2 is null || _point3 is null)
            {
                return "";
            }
            return $"C{_point1.ToXamlString()} {_point2.ToXamlString()} {_point3.ToXamlString()}";
        }

        public override string ToSvgString()
        {
            if (_point1 is null || _point2 is null || _point3 is null)
            {
                return "";
            }
            return $"C{_point1.ToSvgString()} {_point2.ToSvgString()} {_point3.ToSvgString()}";
        }
    }
}
