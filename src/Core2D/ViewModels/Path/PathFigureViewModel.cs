#nullable disable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Text;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path
{
    public partial class PathFigureViewModel : ViewModelBase
    {
        [AutoNotify] private PointShapeViewModel _startPoint;
        [AutoNotify] private ImmutableArray<PathSegmentViewModel> _segments;
        [AutoNotify] private bool _isClosed;

        public PathFigureViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public void GetPoints(IList<PointShapeViewModel> points)
        {
            points.Add(StartPoint);

            foreach (var segment in _segments)
            {
                segment.GetPoints(points);
            }
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= _startPoint.IsDirty();

            foreach (var segment in _segments)
            {
                isDirty |= segment.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _startPoint.Invalidate();

            foreach (var segment in _segments)
            {
                segment.Invalidate();
            }
        }

        public override IDisposable Subscribe(IObserver<(object sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposableStartPoint = default(IDisposable);
            var disposableSegments = default(CompositeDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_startPoint, ref disposableStartPoint, mainDisposable, observer);
            ObserveList(_segments, ref disposableSegments, mainDisposable, observer);

            void Handler(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(StartPoint))
                {
                    ObserveObject(_startPoint, ref disposableStartPoint, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(Segments))
                {
                    ObserveList(_segments, ref disposableSegments, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }

        public string ToXamlString(ImmutableArray<PathSegmentViewModel> segments)
        {
            if (segments.Length == 0)
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            for (int i = 0; i < segments.Length; i++)
            {
                sb.Append(segments[i].ToXamlString());
            }
            return sb.ToString();
        }

        public string ToSvgString(ImmutableArray<PathSegmentViewModel> segments)
        {
            if (segments.Length == 0)
            {
                return string.Empty;
            }
            var sb = new StringBuilder();
            for (int i = 0; i < segments.Length; i++)
            {
                sb.Append(segments[i].ToSvgString());
            }
            return sb.ToString();
        }

        public string ToXamlString()
        {
            return
                (StartPoint is { } ? "M" + StartPoint.ToXamlString() : "")
                + (Segments is { } ? ToXamlString(Segments) : "")
                + (IsClosed ? "z" : "");
        }

        public string ToSvgString()
        {
            return
                (StartPoint is { } ? "M" + StartPoint.ToSvgString() : "")
                + (Segments is { } ? ToSvgString(Segments) : "")
                + (IsClosed ? "z" : "");
        }
    }
}
