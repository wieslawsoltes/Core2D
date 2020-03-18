
namespace Core2D.Data
{
    /// <summary>
    /// Defines value contract.
    /// </summary>
    public interface IValue : IObservableObject
    {
        /// <summary>
        /// Gets or sets value content.
        /// </summary>
        string Content { get; set; }
    }
}
