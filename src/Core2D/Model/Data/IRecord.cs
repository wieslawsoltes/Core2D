using System.Collections.Immutable;

namespace Core2D.Data
{
    /// <summary>
    /// Defines record contract.
    /// </summary>
    public interface IRecord : IObservableObject
    {
        /// <summary>
        /// Gets or sets record values.
        /// </summary>
        ImmutableArray<IValue> Values { get; set; }
    }
}
