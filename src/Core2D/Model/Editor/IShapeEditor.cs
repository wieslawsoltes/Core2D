using System.Collections.Generic;
using Core2D.Path;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    internal interface IShapeEditor
    {
        void BreakPathFigure(PathFigure pathFigure, ShapeStyle style, bool isStroked, bool isFilled, List<BaseShape> result);
        bool BreakPathShape(PathShape pathShape, List<BaseShape> result);
        void BreakShape(BaseShape shape, List<BaseShape> result, List<BaseShape> remove);
    }
}
