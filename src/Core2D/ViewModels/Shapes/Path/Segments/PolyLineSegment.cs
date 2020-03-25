using System;
using System.Collections.Generic;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Poly line path segment.
    /// </summary>
    public class PolyLineSegment : PathPolySegment, IPolyLineSegment
    {
        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => (Points != null) && (Points.Length >= 1) ? "L" + ToString(Points) : "";
    }
}
