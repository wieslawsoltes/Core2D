using System.Threading.Tasks;

namespace Core2D
{
    /// <summary>
    /// Defines text clipboard contract.
    /// </summary>
    public interface ITextClipboard
    {
        /// <summary>
        /// Return true if clipboard contains text string.
        /// </summary>
        /// <returns>True if clipboard contains text string.</returns>
        Task<bool> ContainsText();

        /// <summary>
        /// Get text from clipboard.
        /// </summary>
        /// <returns>The text string.</returns>
        Task<string> GetText();

        /// <summary>
        /// Set clipboard text.
        /// </summary>
        /// <param name="text">The text string.</param>
        Task SetText(string text);
    }
}
