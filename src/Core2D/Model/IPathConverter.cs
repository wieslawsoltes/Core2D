using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;

namespace Core2D
{
    public interface IPathConverter
    {
        PathShape ToPathShape(IEnumerable<BaseShape> shapes);

        PathShape ToPathShape(BaseShape shape);

        PathShape ToStrokePathShape(BaseShape shape);

        PathShape ToFillPathShape(BaseShape shape);

        PathShape ToWindingPathShape(BaseShape shape);

        PathShape Simplify(BaseShape shape);

        PathShape Op(IEnumerable<BaseShape> shapes, PathOp op);

        public PathShape FromSvgPathData(string svgPath, bool isStroked, bool isFilled);

        public string ToSvgPathData(BaseShape shape);
    }
}
