
namespace Core2D.Shapes
{
    /// <summary>
    /// Defines cubic bezier shape contract.
    /// </summary>
    public interface ICubicBezierShape : IBaseShape
    {
        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        IPointShape Point1 { get; set; }

        /// <summary>
        /// Gets or sets first control point.
        /// </summary>
        IPointShape Point2 { get; set; }

        /// <summary>
        /// Gets or sets second control point.
        /// </summary>
        IPointShape Point3 { get; set; }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        IPointShape Point4 { get; set; }
    }
}
