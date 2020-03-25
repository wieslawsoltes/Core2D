
namespace Core2D.Scripting
{
    /// <summary>
    /// Defines script contract.
    /// </summary>
    public interface IScript : IObservableObject
    {
        /// <summary>
        /// Gets or sets script code.
        /// </summary>
        string Code { get; set; }
    }
}
