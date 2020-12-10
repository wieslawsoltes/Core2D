using System;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Model.Renderer.Nodes
{
    public interface IDrawNodeFactory
    {
        IFillDrawNode CreateFillDrawNode(double x, double y, double width, double height, BaseColorViewModel colorViewModel);

        IGridDrawNode CreateGridDrawNode(IGrid grid, double x, double y, double width, double height);

        IPointDrawNode CreatePointDrawNode(PointShapeViewModel point, ShapeStyleViewModel pointStyleViewModel, double pointSize);

        ILineDrawNode CreateLineDrawNode(LineShapeViewModel line, ShapeStyleViewModel style);

        IRectangleDrawNode CreateRectangleDrawNode(RectangleShapeViewModel rectangle, ShapeStyleViewModel style);

        IEllipseDrawNode CreateEllipseDrawNode(EllipseShapeViewModel ellipse, ShapeStyleViewModel style);

        IArcDrawNode CreateArcDrawNode(ArcShapeViewModelViewModel arc, ShapeStyleViewModel style);

        ICubicBezierDrawNode CreateCubicBezierDrawNode(CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel style);

        IQuadraticBezierDrawNode CreateQuadraticBezierDrawNode(QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel style);

        ITextDrawNode CreateTextDrawNode(TextShapeViewModel text, ShapeStyleViewModel style);

        IImageDrawNode CreateImageDrawNode(ImageShapeViewModel image, ShapeStyleViewModel style, IImageCache imageCache, ICache<string, IDisposable> bitmapCache);

        IPathDrawNode CreatePathDrawNode(PathShapeViewModel path, ShapeStyleViewModel style);
    }
}
