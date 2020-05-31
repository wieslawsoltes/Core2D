using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D
{
    /// <summary>
    /// Define svg converter contract.
    /// </summary>
    public interface ISvgConverter
    {
        /// <summary>
        /// Converts svg file to shapes.
        /// </summary>
        /// <param name="path">The svg path.</param>
        /// <returns>The converted shapes.</returns>
        IList<IBaseShape> Convert(string path);

        /// <summary>
        /// Converts svg text to shapes.
        /// </summary>
        /// <param name="text">The svg text.</param>
        /// <returns>The converted shapes.</returns>
        IList<IBaseShape> FromString(string text);
    }
}
