
namespace Core2D.Shapes
{
    /// <summary>
    /// Defines arc shape contract.
    /// </summary>
    public interface IArcShape : IBaseShape
    {
        /// <summary>
        /// Gets or sets top-left corner point.
        /// </summary>
        IPointShape Point1 { get; set; }

        /// <summary>
        /// Gets or sets bottom-right corner point.
        /// </summary>
        IPointShape Point2 { get; set; }

        /// <summary>
        /// Gets or sets point used to calculate arc start angle.
        /// </summary>
        IPointShape Point3 { get; set; }

        /// <summary>
        /// Gets or sets point used to calculate arc end angle.
        /// </summary>
        IPointShape Point4 { get; set; }
    }
}
