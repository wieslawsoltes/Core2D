
namespace Core2D
{
    /// <summary>
    /// Defines svg exporter contract.
    /// </summary>
    public interface ISvgExporter
    {
        /// <summary>
        /// Creates the svg string.
        /// </summary>
        /// <param name="item">The object to export.</param>
        /// <param name="width">The svg width.</param>
        /// <param name="height">The svg height.</param>
        /// <returns>The new instance of the svg string.</returns>
        string Create(object item, double width, double height);
    }
}
