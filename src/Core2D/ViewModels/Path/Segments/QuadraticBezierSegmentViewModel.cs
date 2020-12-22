#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path.Segments
{
    public partial class QuadraticBezierSegmentViewModel : PathSegmentViewModel
    {
        [AutoNotify] private PointShapeViewModel _point1;
        [AutoNotify] private PointShapeViewModel _point2;

        public QuadraticBezierSegmentViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            points.Add(_point1);
            points.Add(_point2);
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= _point1.IsDirty();
            isDirty |= _point2.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _point1.Invalidate();
            _point2.Invalidate();
        }

        public override IDisposable Subscribe(IObserver<(object sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposablePoint1 = default(IDisposable);
            var disposablePoint2 = default(IDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_point1, ref disposablePoint1, mainDisposable, observer);
            ObserveObject(_point2, ref disposablePoint2, mainDisposable, observer);

            void Handler(object sender, PropertyChangedEventArgs e) 
            {
                if (e.PropertyName == nameof(Point1))
                {
                    ObserveObject(_point1, ref disposablePoint1, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(Point2))
                {
                    ObserveObject(_point2, ref disposablePoint2, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }

        public override string ToXamlString()
            => $"Q{Point1.ToXamlString()} {Point2.ToXamlString()}";

        public override string ToSvgString()
            => $"Q{Point1.ToSvgString()} {Point2.ToSvgString()}";
    }
}
