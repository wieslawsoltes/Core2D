using System.Collections.Immutable;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// Defines path poly segment contract.
    /// </summary>
    public interface IPathPolySegment : IPathSegment
    {
        /// <summary>
        /// Gets or sets points array.
        /// </summary>
        ImmutableArray<IPointShape> Points { get; set; }
    }
}
