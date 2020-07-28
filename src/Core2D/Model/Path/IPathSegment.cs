using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// Defines path segment contract.
    /// </summary>
    public interface IPathSegment : IObservableObject, IStringExporter
    {
        /// <summary>
        /// Gets or sets flag indicating whether segment is stroked.
        /// </summary>
        bool IsStroked { get; set; }

        /// <summary>
        /// Get all points in the shape.
        /// </summary>
        /// <param name="points">The points list.</param>
        void GetPoints(IList<IPointShape> points);
    }
}
