using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    public class PointSelection
    {
        private readonly LayerContainerViewModel _layer;
        private readonly PointShapeViewModel _shapeViewModel;
        private readonly ShapeStyleViewModel _styleViewModel;

        public PointSelection(LayerContainerViewModel layer, PointShapeViewModel shape, ShapeStyleViewModel style)
        {
            _layer = layer;
            _shapeViewModel = shape;
            _styleViewModel = style;
        }
    }
}
