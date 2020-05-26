using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// Path figure.
    /// </summary>
    public class PathFigure : ObservableObject, IPathFigure
    {
        private IPointShape _startPoint;
        private ImmutableArray<IPathSegment> _segments;
        private bool _isFilled;
        private bool _isClosed;

        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        public IPointShape StartPoint
        {
            get => _startPoint;
            set => Update(ref _startPoint, value);
        }

        /// <summary>
        /// Gets or sets segments collection.
        /// </summary>
        public ImmutableArray<IPathSegment> Segments
        {
            get => _segments;
            set => Update(ref _segments, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether path is filled.
        /// </summary>
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        /// <summary>
        /// Gets or sets flag indicating whether path is closed.
        /// </summary>
        public bool IsClosed
        {
            get => _isClosed;
            set => Update(ref _isClosed, value);
        }

        /// <inheritdoc/>
        public void GetPoints(IList<IPointShape> points)
        {
            points.Add(StartPoint);

            foreach (var segment in Segments)
            {
                segment.GetPoints(points);
            }
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public string ToXamlString(ImmutableArray<IPathSegment> segments)
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

        public string ToSvgString(ImmutableArray<IPathSegment> segments)
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

        /// <inheritdoc/>
        public string ToXamlString()
        {
            return
                (StartPoint != null ? "M" + StartPoint.ToXamlString() : "")
                + (Segments != null ? ToXamlString(Segments) : "")
                + (IsClosed ? "z" : "");
        }

        /// <inheritdoc/>
        public string ToSvgString()
        {
            return
                (StartPoint != null ? "M" + StartPoint.ToSvgString() : "")
                + (Segments != null ? ToSvgString(Segments) : "")
                + (IsClosed? "z" : "");
        }

        /// <summary>
        /// Check whether the <see cref="StartPoint"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeStartPoint() => _startPoint != null;

        /// <summary>
        /// Check whether the <see cref="Segments"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeSegments() => true;

        /// <summary>
        /// Check whether the <see cref="IsFilled"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsFilled() => _isFilled != default;

        /// <summary>
        /// Check whether the <see cref="IsClosed"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeIsClosed() => _isClosed != default;
    }
}
