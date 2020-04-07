using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;

namespace Core2D.Interfaces
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
    }
}
