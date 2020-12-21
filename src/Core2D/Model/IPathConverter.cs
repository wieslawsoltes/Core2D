using System.Collections.Generic;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
#nullable disable
namespace Core2D.Model
{
    public interface IPathConverter
    {
        PathShapeViewModel ToPathShape(IEnumerable<BaseShapeViewModel> shapes);

        PathShapeViewModel ToPathShape(BaseShapeViewModel shape);

        PathShapeViewModel ToStrokePathShape(BaseShapeViewModel shape);

        PathShapeViewModel ToFillPathShape(BaseShapeViewModel shape);

        PathShapeViewModel ToWindingPathShape(BaseShapeViewModel shape);

        PathShapeViewModel Simplify(BaseShapeViewModel shape);

        PathShapeViewModel Op(IEnumerable<BaseShapeViewModel> shapes, PathOp op);

        public PathShapeViewModel FromSvgPathData(string svgPath, bool isStroked, bool isFilled);

        public string ToSvgPathData(BaseShapeViewModel shape);
    }
}
