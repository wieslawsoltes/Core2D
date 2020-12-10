using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools.Selection
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
