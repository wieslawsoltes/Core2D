using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    /// <summary>
    /// Helper class for <see cref="PathShape"/> shape selection.
    /// </summary>
    public class ToolPathSelection
    {
        private readonly LayerContainer _layer;
        private readonly PathShape _path;
        private readonly ShapeStyle _style;

        /// <summary>
        /// Initialize new instance of <see cref="ToolPathSelection"/> class.
        /// </summary>
        /// <param name="layer">The selection shapes layer.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="style">The selection shapes style.</param>
        public ToolPathSelection(LayerContainer layer, PathShape shape, ShapeStyle style)
        {
            _layer = layer;
            _path = shape;
            _style = style;
        }
    }
}
