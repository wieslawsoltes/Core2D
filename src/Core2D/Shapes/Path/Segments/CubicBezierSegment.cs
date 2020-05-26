using System;
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Cubic bezier path segment.
    /// </summary>
    public class CubicBezierSegment : PathSegment, ICubicBezierSegment
    {
        private IPointShape _point1;
        private IPointShape _point2;
        private IPointShape _point3;

        /// <inheritdoc/>
        public IPointShape Point1
        {
            get => _point1;
            set => Update(ref _point1, value);
        }

        /// <inheritdoc/>
        public IPointShape Point2
        {
            get => _point2;
            set => Update(ref _point2, value);
        }

        /// <inheritdoc/>
        public IPointShape Point3
        {
            get => _point3;
            set => Update(ref _point3, value);
        }

        /// <inheritdoc/>
        public override void GetPoints(IList<IPointShape> points)
        {
            points.Add(Point1);
            points.Add(Point2);
            points.Add(Point3);
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string ToXamlString()
            => $"C{Point1.ToXamlString()} {Point2.ToXamlString()} {Point3.ToXamlString()}";

        /// <inheritdoc/>
        public override string ToSvgString()
            => $"C{Point1.ToSvgString()} {Point2.ToSvgString()} {Point3.ToSvgString()}";

        /// <summary>
        /// Check whether the <see cref="Point1"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint1() => _point1 != null;

        /// <summary>
        /// Check whether the <see cref="Point2"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint2() => _point2 != null;

        /// <summary>
        /// Check whether the <see cref="Point3"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializePoint3() => _point3 != null;
    }
}
