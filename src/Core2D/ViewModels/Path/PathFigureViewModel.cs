using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path
{
    public partial class PathFigureViewModel : ViewModelBase
    {
        [AutoNotify] private PointShapeViewModel _startPoint;
        [AutoNotify] private ImmutableArray<PathSegmentViewModel> _segments;
        [AutoNotify] private bool _isClosed;

        public void GetPoints(IList<PointShapeViewModel> points)
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
    }
}
