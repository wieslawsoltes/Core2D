using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Core2D.Shapes;

namespace Core2D.Path
{
    public class PathFigure : ObservableObject
    {
        private PointShape _startPoint;
        private ImmutableArray<PathSegment> _segments;
        private bool _isClosed;

        public PointShape StartPoint
        {
            get => _startPoint;
            set => RaiseAndSetIfChanged(ref _startPoint, value);
        }

        public ImmutableArray<PathSegment> Segments
        {
            get => _segments;
            set => RaiseAndSetIfChanged(ref _segments, value);
        }

        public bool IsClosed
        {
            get => _isClosed;
            set => RaiseAndSetIfChanged(ref _isClosed, value);
        }

        public void GetPoints(IList<PointShape> points)
        {
            points.Add(StartPoint);

            foreach (var segment in Segments)
            {
                segment.GetPoints(points);
            }
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= StartPoint.IsDirty();

            foreach (var segment in Segments)
            {
                isDirty |= segment.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            StartPoint.Invalidate();

            foreach (var segment in Segments)
            {
                segment.Invalidate();
            }
        }

        public string ToXamlString(ImmutableArray<PathSegment> segments)
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

        public string ToSvgString(ImmutableArray<PathSegment> segments)
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
                (StartPoint != null ? "M" + StartPoint.ToXamlString() : "")
                + (Segments != null ? ToXamlString(Segments) : "")
                + (IsClosed ? "z" : "");
        }

        public string ToSvgString()
        {
            return
                (StartPoint != null ? "M" + StartPoint.ToSvgString() : "")
                + (Segments != null ? ToSvgString(Segments) : "")
                + (IsClosed ? "z" : "");
        }

        public virtual bool ShouldSerializeStartPoint() => _startPoint != null;

        public virtual bool ShouldSerializeSegments() => true;

        public virtual bool ShouldSerializeIsClosed() => _isClosed != default;
    }
}
