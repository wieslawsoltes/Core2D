
namespace Core2D
{
    /// <summary>
    /// Defines xaml exporter contract.
    /// </summary>
    public interface IXamlExporter
    {
        /// <summary>
        /// Creates the xaml string.
        /// </summary>
        /// <param name="item">The object to export.</param>
        /// <param name="key">The resouce key.</param>
        /// <returns>The new instance of the xaml string.</returns>
        string Create(object item, string key);
    }
}
