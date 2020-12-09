using System;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    internal class AvaloniaDrawNodeFactory : IDrawNodeFactory
    {
        public IFillDrawNode CreateFillDrawNode(double x, double y, double width, double height, BaseColorViewModel colorViewModel)
        {
            return new FillDrawNode(x, y, width, height, colorViewModel);
        }

        public IGridDrawNode CreateGridDrawNode(IGrid grid, double x, double y, double width, double height)
        {
            return new GridDrawNode(grid, x, y, width, height);
        }

        public IPointDrawNode CreatePointDrawNode(PointShapeViewModel point, ShapeStyleViewModel pointStyleViewModel, double pointSize)
        {
            return new PointDrawNode(point, pointStyleViewModel, pointSize);
        }

        public ILineDrawNode CreateLineDrawNode(LineShapeViewModel line, ShapeStyleViewModel styleViewModel)
        {
            return new LineDrawNode(line, styleViewModel);
        }

        public IRectangleDrawNode CreateRectangleDrawNode(RectangleShapeViewModel rectangle, ShapeStyleViewModel styleViewModel)
        {
            return new RectangleDrawNode(rectangle, styleViewModel);
        }

        public IEllipseDrawNode CreateEllipseDrawNode(EllipseShapeViewModel ellipse, ShapeStyleViewModel styleViewModel)
        {
            return new EllipseDrawNode(ellipse, styleViewModel);
        }

        public IArcDrawNode CreateArcDrawNode(ArcShapeViewModelViewModel arc, ShapeStyleViewModel styleViewModel)
        {
            return new ArcDrawNode(arc, styleViewModel);
        }

        public ICubicBezierDrawNode CreateCubicBezierDrawNode(CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel styleViewModel)
        {
            return new CubicBezierDrawNode(cubicBezier, styleViewModel);
        }

        public IQuadraticBezierDrawNode CreateQuadraticBezierDrawNode(QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel styleViewModel)
        {
            return new QuadraticBezierDrawNode(quadraticBezier, styleViewModel);
        }

        public ITextDrawNode CreateTextDrawNode(TextShapeViewModel text, ShapeStyleViewModel styleViewModel)
        {
            return new TextDrawNode(text, styleViewModel);
        }

        public IImageDrawNode CreateImageDrawNode(ImageShapeViewModel image, ShapeStyleViewModel styleViewModel, IImageCache imageCache, ICache<string, IDisposable> bitmapCache)
        {
            return new ImageDrawNode(image, styleViewModel, imageCache, bitmapCache);
        }

        public IPathDrawNode CreatePathDrawNode(PathShapeViewModel path, ShapeStyleViewModel styleViewModel)
        {
            return new PathDrawNode(path, styleViewModel);
        }
    }
}
