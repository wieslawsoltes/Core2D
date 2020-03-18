using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    /// <summary>
    /// Defines line path segment contract.
    /// </summary>
    public interface ILineSegment : IPathSegment
    {
        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        IPointShape Point { get; set; }
    }
}
