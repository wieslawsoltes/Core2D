#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Disposables;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path.Segments
{
    public partial class LineSegmentViewModel : PathSegmentViewModel
    {
        [AutoNotify] private PointShapeViewModel _point;

        public LineSegmentViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            points.Add(_point);
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= _point.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _point.Invalidate();
        }

        public override IDisposable Subscribe(IObserver<(object sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposablePoint = default(IDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_point, ref disposablePoint, mainDisposable, observer);
 
            void Handler(object sender, PropertyChangedEventArgs e) 
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
            => $"L{Point.ToXamlString()}";

        public override string ToSvgString()
            => $"L{Point.ToSvgString()}";
    }
}
