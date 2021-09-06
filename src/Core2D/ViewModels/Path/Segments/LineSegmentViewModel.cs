#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path.Segments
{
    public partial class LineSegmentViewModel : PathSegmentViewModel
    {
        [AutoNotify] private PointShapeViewModel? _point;

        public LineSegmentViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            return new LineSegmentViewModel(ServiceProvider)
            {
                Name = Name,
                IsStroked = IsStroked,
                Point = (PointShapeViewModel?)_point?.Copy(shared),
            };
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            if (_point is null)
            {
                return;
            }

            points.Add(_point);
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (_point != null)
            {
                isDirty |= _point.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _point?.Invalidate();
        }

        public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposablePoint = default(IDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_point, ref disposablePoint, mainDisposable, observer);

            void Handler(object? sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(Point))
                {
                    ObserveObject(_point, ref disposablePoint, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }

        public override string ToXamlString()
        {
            if (_point is null)
            {
                return "";
            }
            return $"L{_point.ToXamlString()}";
        }

        public override string ToSvgString()
        {
            if (_point is null)
            {
                return "";
            }
            return $"L{_point.ToSvgString()}";
        }
    }
}
