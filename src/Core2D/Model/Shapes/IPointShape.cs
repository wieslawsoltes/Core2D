using Core2D.Renderer;

namespace Core2D.Shapes
{
    /// <summary>
    /// Defines point shape contract.
    /// </summary>
    public interface IPointShape : IBaseShape, IStringExporter
    {
        /// <summary>
        /// Gets or sets X coordinate of point.
        /// </summary>
        double X { get; set; }

        /// <summary>
        /// Gets or sets Y coordinate of point.
        /// </summary>
        double Y { get; set; }

        /// <summary>
        /// Gets or sets point alignment.
        /// </summary>
        PointAlignment Alignment { get; set; }
    }
}
