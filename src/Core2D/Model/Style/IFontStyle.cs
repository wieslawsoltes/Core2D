
namespace Core2D.Style
{
    /// <summary>
    /// Define font style contract.
    /// </summary>
    public interface IFontStyle : IObservableObject
    {
        /// <summary>
        /// Get or sets font style flags.
        /// </summary>
        FontStyleFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets <see cref="FontStyleFlags.Regular"/> flag.
        /// </summary>
        bool Regular { get; set; }

        /// <summary>
        /// Gets or sets <see cref="FontStyleFlags.Bold"/> flag.
        /// </summary>
        bool Bold { get; set; }

        /// <summary>
        /// Gets or sets <see cref="FontStyleFlags.Italic"/> flag.
        /// </summary>
        bool Italic { get; set; }
    }
}
