using System.Collections.Generic;
using Core2D.Path;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor
{
    internal interface IShapeEditor
    {
        void BreakPathFigure(IPathFigure pathFigure, IShapeStyle style, bool isStroked, bool isFilled, List<IBaseShape> result);
        bool BreakPathShape(IPathShape pathShape, List<IBaseShape> result);
        void BreakShape(IBaseShape shape, List<IBaseShape> result, List<IBaseShape> remove);
    }
}
