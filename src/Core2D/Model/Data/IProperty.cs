
namespace Core2D.Data
{
    /// <summary>
    /// Defines property contract.
    /// </summary>
    public interface IProperty : IObservableObject
    {
        /// <summary>
        /// Gets or sets property value.
        /// </summary>
        string? Value { get; set; }
    }
}
