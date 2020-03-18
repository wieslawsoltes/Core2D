
namespace Core2D.Path
{
    /// <summary>
    /// Defines path size contract.
    /// </summary>
    public interface IPathSize : IObservableObject
    {
        /// <summary>
        /// Gets or sets width value.
        /// </summary>
        double Width { get; set; }

        /// <summary>
        /// Gets or sets height value.
        /// </summary>
        double Height { get; set; }
    }
}
