#nullable enable
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
        [AutoNotify] private PointShapeViewModel? _startPoint;
        [AutoNotify] private ImmutableArray<PathSegmentViewModel> _segments;
        [AutoNotify] private bool _isClosed;

        public PathFigureViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var segments = _segments.Copy(shared).ToImmutable();

            return new PathFigureViewModel(ServiceProvider)
            {
                Name = Name,
                StartPoint = (PointShapeViewModel?)_startPoint?.Copy(shared),
                Segments = segments,
                IsClosed = IsClosed
            };
        }

        public void GetPoints(IList<PointShapeViewModel> points)
        {
            if (_startPoint == null)
            {
                return;
            }

            points.Add(_startPoint);

            foreach (var segment in _segments)
            {
                segment.GetPoints(points);
            }
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (_startPoint != null)
            {
                isDirty |= _startPoint.IsDirty();
            }

            foreach (var segment in _segments)
            {
                isDirty |= segment.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _startPoint?.Invalidate();

            foreach (var segment in _segments)
            {
                segment.Invalidate();
            }
        }

        public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposableStartPoint = default(IDisposable);
            var disposableSegments = default(CompositeDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_startPoint, ref disposableStartPoint, mainDisposable, observer);
            ObserveList(_segments, ref disposableSegments, mainDisposable, observer);

            void Handler(object? sender, PropertyChangedEventArgs e)
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

        private string ToXamlString(ImmutableArray<PathSegmentViewModel> segments)
        {
            if (segments.Length == 0)
            {
                return "";
            }
            var sb = new StringBuilder();
            foreach (var segment in segments)
            {
                sb.Append(segment.ToXamlString());
            }
            return sb.ToString();
        }

        private string ToSvgString(ImmutableArray<PathSegmentViewModel> segments)
        {
            if (segments.Length == 0)
            {
                return "";
            }
            var sb = new StringBuilder();
            foreach (var segment in segments)
            {
                sb.Append(segment.ToSvgString());
            }
            return sb.ToString();
        }

        public string ToXamlString()
        {
            if (_startPoint == null)
            {
                return "";
            }
            return $"M{_startPoint.ToXamlString()}{ToXamlString(_segments)}{(_isClosed ? "z" : "")}";
        }

        public string ToSvgString()
        {
            if (_startPoint == null)
            {
                return "";
            }
            return $"M{_startPoint.ToSvgString()}{ToSvgString(_segments)}{(_isClosed ? "z" : "")}";
        }
    }
}
