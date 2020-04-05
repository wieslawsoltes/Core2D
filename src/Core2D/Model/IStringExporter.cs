
namespace Core2D
{
    public interface IStringExporter
    {
        /// <summary>
        /// Creates xaml path data string.
        /// </summary>
        /// <returns>The xaml path data string.</returns>
        string ToXamlString();

        /// <summary>
        /// Creates svg path data string.
        /// </summary>
        /// <returns>The svg path data string.</returns>
        string ToSvgString();
    }
}
