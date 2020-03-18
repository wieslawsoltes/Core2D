using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// Defines path segment contract.
    /// </summary>
    public interface IPathSegment : IObservableObject
    {
        /// <summary>
        /// Gets or sets flag indicating whether segment is stroked.
        /// </summary>
        bool IsStroked { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether segment is smooth join.
        /// </summary>
        bool IsSmoothJoin { get; set; }

        /// <summary>
        /// Get all points in the segment.
        /// </summary>
        /// <returns>All points in the segment.</returns>
        IEnumerable<IPointShape> GetPoints();
    }
}
