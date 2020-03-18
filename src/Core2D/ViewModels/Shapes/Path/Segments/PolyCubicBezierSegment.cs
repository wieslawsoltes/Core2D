using System;
using System.Collections.Generic;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Poly cubic bezier path segment.
    /// </summary>
    public class PolyCubicBezierSegment : PathPolySegment, IPolyCubicBezierSegment
    {
        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override string ToString() => (Points != null) && (Points.Length >= 1) ? "C" + ToString(Points) : "";
    }
}
