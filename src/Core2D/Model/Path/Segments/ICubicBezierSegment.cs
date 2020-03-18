using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Defines cubic bezier path segment contract.
    /// </summary>
    public interface ICubicBezierSegment : IPathSegment
    {
        /// <summary>
        /// Gets or sets first control point.
        /// </summary>
        IPointShape Point1 { get; set; }

        /// <summary>
        /// Gets or sets second control point.
        /// </summary>
        IPointShape Point2 { get; set; }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        IPointShape Point3 { get; set; }
    }
}
