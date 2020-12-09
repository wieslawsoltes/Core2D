using System;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    public interface IDrawNodeFactory
    {
        IFillDrawNode CreateFillDrawNode(double x, double y, double width, double height, BaseColorViewModel colorViewModel);

        IGridDrawNode CreateGridDrawNode(IGrid grid, double x, double y, double width, double height);

        IPointDrawNode CreatePointDrawNode(PointShapeViewModel point, ShapeStyleViewModel pointStyleViewModel, double pointSize);

        ILineDrawNode CreateLineDrawNode(LineShapeViewModel line, ShapeStyleViewModel styleViewModel);

        IRectangleDrawNode CreateRectangleDrawNode(RectangleShapeViewModel rectangle, ShapeStyleViewModel styleViewModel);

        IEllipseDrawNode CreateEllipseDrawNode(EllipseShapeViewModel ellipse, ShapeStyleViewModel styleViewModel);

        IArcDrawNode CreateArcDrawNode(ArcShapeViewModelViewModel arc, ShapeStyleViewModel styleViewModel);

        ICubicBezierDrawNode CreateCubicBezierDrawNode(CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel styleViewModel);

        IQuadraticBezierDrawNode CreateQuadraticBezierDrawNode(QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel styleViewModel);

        ITextDrawNode CreateTextDrawNode(TextShapeViewModel text, ShapeStyleViewModel styleViewModel);

        IImageDrawNode CreateImageDrawNode(ImageShapeViewModel image, ShapeStyleViewModel styleViewModel, IImageCache imageCache, ICache<string, IDisposable> bitmapCache);

        IPathDrawNode CreatePathDrawNode(PathShapeViewModel path, ShapeStyleViewModel styleViewModel);
    }
}
