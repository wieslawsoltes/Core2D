using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    public class ToolPointSelection
    {
        private readonly LayerContainer _layer;
        private readonly PointShape _shape;
        private readonly ShapeStyle _style;

        public ToolPointSelection(LayerContainer layer, PointShape shape, ShapeStyle style)
        {
            _layer = layer;
            _shape = shape;
            _style = style;
        }
    }
}
