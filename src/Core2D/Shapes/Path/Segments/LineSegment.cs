using System;
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Line path segment.
    /// </summary>
    public class LineSegment : PathSegment, ILineSegment
    {
        private IPointShape _point;

        /// <inheritdoc/>
        public IPointShape Point
        {
            get => _point;
            set => Update(ref _point, value);
        }

        /// <inheritdoc/>
        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(Point);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
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
