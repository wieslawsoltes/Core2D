using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    public class PathSelection
    {
        private readonly LayerContainerViewModel _layer;
        private readonly PathShapeViewModel _path;
        private readonly ShapeStyleViewModel _styleViewModel;

        public PathSelection(LayerContainerViewModel layer, PathShapeViewModel shapeViewModel, ShapeStyleViewModel styleViewModel)
        {
            _layer = layer;
            _path = shapeViewModel;
            _styleViewModel = styleViewModel;
        }
    }
}
