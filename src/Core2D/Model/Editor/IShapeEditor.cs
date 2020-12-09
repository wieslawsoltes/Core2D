using System.Collections.Generic;
using Core2D.Path;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    internal interface IShapeEditor
    {
        void BreakPathFigure(PathFigureViewModel pathFigureViewModel, ShapeStyleViewModel styleViewModel, bool isStroked, bool isFilled, List<BaseShapeViewModel> result);
        bool BreakPathShape(PathShapeViewModel pathShapeViewModel, List<BaseShapeViewModel> result);
        void BreakShape(BaseShapeViewModel shapeViewModel, List<BaseShapeViewModel> result, List<BaseShapeViewModel> remove);
    }
}
