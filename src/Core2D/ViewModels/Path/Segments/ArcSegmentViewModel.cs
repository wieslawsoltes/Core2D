#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reactive.Disposables;
using Core2D.Model.Path;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path.Segments
{
    public partial class ArcSegmentViewModel : PathSegmentViewModel
    {
        public static SweepDirection[] SweepDirectionValues { get; } = (SweepDirection[])Enum.GetValues(typeof(SweepDirection));

        [AutoNotify] private PointShapeViewModel _point;
        [AutoNotify] private PathSizeViewModel _size;
        [AutoNotify] private double _rotationAngle;
        [AutoNotify] private bool _isLargeArc;
        [AutoNotify] private SweepDirection _sweepDirection;

        public ArcSegmentViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            points.Add(Point);
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Point.IsDirty();
            isDirty |= Size.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            Point.Invalidate();
            Size.Invalidate();
        }

        public override IDisposable Subscribe(IObserver<(object sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposablePoint = default(IDisposable);
            var disposableSize = default(IDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_point, ref disposablePoint, mainDisposable, observer);
            ObserveObject(_size, ref disposableSize, mainDisposable, observer);

            void Handler(object sender, PropertyChangedEventArgs e) 
            {
                if (e.PropertyName == nameof(Point))
                {
                    ObserveObject(_point, ref disposablePoint, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(Size))
                {
                    ObserveObject(_size, ref disposableSize, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }

        public override string ToXamlString()
            => $"A{Size.ToXamlString()} {RotationAngle.ToString(CultureInfo.InvariantCulture)} {(IsLargeArc ? "1" : "0")} {(SweepDirection == SweepDirection.Clockwise ? "1" : "0")} {Point.ToXamlString()}";

        public override string ToSvgString()
            => $"A{Size.ToSvgString()} {RotationAngle.ToString(CultureInfo.InvariantCulture)} {(IsLargeArc ? "1" : "0")} {(SweepDirection == SweepDirection.Clockwise ? "1" : "0")} {Point.ToSvgString()}";
    }
}
