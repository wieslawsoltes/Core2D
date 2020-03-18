using System.Collections.Immutable;

namespace Core2D.Shapes
{
    /// <summary>
    /// Defines point shape contract.
    /// </summary>
    public interface IConnectableShape : IBaseShape
    {
        /// <summary>
        /// Gets or sets connectors collection.
        /// </summary>
        ImmutableArray<IPointShape> Connectors { get; set; }
    }
}
