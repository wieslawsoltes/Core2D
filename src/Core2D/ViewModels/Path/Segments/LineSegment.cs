using System;
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Line path segment.
    /// </summary>
    public class LineSegment : PathSegment
    {
        private PointShape _point;

        /// <inheritdoc/>
        public PointShape Point
        {
            get => _point;
            set => Update(ref _point, value);
        }

        /// <inheritdoc/>
        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(Point);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Point.IsDirty();

            return isDirty;
        }

        /// <inheritdoc/>
        public override void Invalidate()
        {
            base.Invalidate();

            Point.Invalidate();
        }

        /// <inheritdoc/>
        public override string ToXamlString()
            => $"L{Point.ToXamlString()}";

        /// <inheritdoc/>
        public override string ToSvgString()
            => $"L{Point.ToSvgString()}";

        /// <summary>
        /// Check whether the <see cref="Point"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint() => _point != null;
    }
}
