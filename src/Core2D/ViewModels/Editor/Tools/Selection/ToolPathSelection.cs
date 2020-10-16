using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    public class ToolPathSelection
    {
        private readonly LayerContainer _layer;
        private readonly PathShape _path;
        private readonly ShapeStyle _style;

        public ToolPathSelection(LayerContainer layer, PathShape shape, ShapeStyle style)
        {
            _layer = layer;
            _path = shape;
            _style = style;
        }
    }
}
