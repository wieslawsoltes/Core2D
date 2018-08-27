// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Core2D.Attributes;
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
        [Content]
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

        /// <summary>
        /// Initializes a new instance of the <see cref="PathFigure"/> class.
        /// </summary>
        public PathFigure()
        {
            StartPoint = PointShape.Create();
            Segments = ImmutableArray.Create<IPathSegment>();
        }

        /// <inheritdoc/>
        public IEnumerable<IPointShape> GetPoints()
        {
            yield return StartPoint;

            foreach (var point in Segments.SelectMany(s => s.GetPoints()))
            {
                yield return point;
            }
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new <see cref="PathFigure"/> instance.
        /// </summary>
        /// <param name="startPoint">The start point.</param>
        /// <param name="isFilled">The flag indicating whether path is filled.</param>
        /// <param name="isClosed">The flag indicating whether path is closed.</param>
        /// <returns>The new instance of the <see cref="PathFigure"/> class.</returns>
        public static IPathFigure Create(IPointShape startPoint, bool isFilled = true, bool isClosed = true)
        {
            return new PathFigure()
            {
                StartPoint = startPoint,
                IsFilled = isFilled,
                IsClosed = isClosed
            };
        }

        /// <summary>
        /// Creates a string representation of segments collection.
        /// </summary>
        /// <param name="segments">The segments collection.</param>
        /// <returns>A string representation of segments collection.</returns>
        public string ToString(ImmutableArray<IPathSegment> segments)
        {
            if (segments.Length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < segments.Length; i++)
            {
                sb.Append(segments[i].ToString());
            }
            return sb.ToString();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return
                (StartPoint != null ? "M" + StartPoint.ToString() : "")
                + (Segments != null ? ToString(Segments) : "")
                + (IsClosed ? "z" : "");
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
        public virtual bool ShouldSerializeSegments() => _segments.IsEmpty == false;

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
