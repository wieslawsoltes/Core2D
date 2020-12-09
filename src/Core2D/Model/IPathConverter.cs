using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;

namespace Core2D
{
    public interface IPathConverter
    {
        PathShapeViewModel ToPathShape(IEnumerable<BaseShapeViewModel> shapes);

        PathShapeViewModel ToPathShape(BaseShapeViewModel shapeViewModel);

        PathShapeViewModel ToStrokePathShape(BaseShapeViewModel shapeViewModel);

        PathShapeViewModel ToFillPathShape(BaseShapeViewModel shapeViewModel);

        PathShapeViewModel ToWindingPathShape(BaseShapeViewModel shapeViewModel);

        PathShapeViewModel Simplify(BaseShapeViewModel shapeViewModel);

        PathShapeViewModel Op(IEnumerable<BaseShapeViewModel> shapes, PathOp op);

        public PathShapeViewModel FromSvgPathData(string svgPath, bool isStroked, bool isFilled);

        public string ToSvgPathData(BaseShapeViewModel shapeViewModel);
    }
}
