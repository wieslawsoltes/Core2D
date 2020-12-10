using System;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer.SkiaSharp
{
    internal class SkiaSharpDrawNodeFactory : IDrawNodeFactory
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

        public ILineDrawNode CreateLineDrawNode(LineShapeViewModel line, ShapeStyleViewModel style)
        {
            return new LineDrawNode(line, style);
        }

        public IRectangleDrawNode CreateRectangleDrawNode(RectangleShapeViewModel rectangle, ShapeStyleViewModel style)
        {
            return new RectangleDrawNode(rectangle, style);
        }

        public IEllipseDrawNode CreateEllipseDrawNode(EllipseShapeViewModel ellipse, ShapeStyleViewModel style)
        {
            return new EllipseDrawNode(ellipse, style);
        }

        public IArcDrawNode CreateArcDrawNode(ArcShapeViewModelViewModel arc, ShapeStyleViewModel style)
        {
            return new ArcDrawNode(arc, style);
        }

        public ICubicBezierDrawNode CreateCubicBezierDrawNode(CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel style)
        {
            return new CubicBezierDrawNode(cubicBezier, style);
        }

        public IQuadraticBezierDrawNode CreateQuadraticBezierDrawNode(QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel style)
        {
            return new QuadraticBezierDrawNode(quadraticBezier, style);
        }

        public ITextDrawNode CreateTextDrawNode(TextShapeViewModel text, ShapeStyleViewModel style)
        {
            return new TextDrawNode(text, style);
        }

        public IImageDrawNode CreateImageDrawNode(ImageShapeViewModel image, ShapeStyleViewModel style, IImageCache imageCache, ICache<string, IDisposable> bitmapCache)
        {
            return new ImageDrawNode(image, style, imageCache, bitmapCache);
        }

        public IPathDrawNode CreatePathDrawNode(PathShapeViewModel path, ShapeStyleViewModel style)
        {
            return new PathDrawNode(path, style);
        }
    }
}
