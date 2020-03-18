
namespace Core2D.Shapes
{
    /// <summary>
    /// Defines line shape contract.
    /// </summary>
    public interface ILineShape : IBaseShape
    {
        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        IPointShape Start { get; set; }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        IPointShape End { get; set; }
    }
}
