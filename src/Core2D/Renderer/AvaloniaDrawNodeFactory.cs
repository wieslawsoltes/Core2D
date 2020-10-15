using System;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    internal class AvaloniaDrawNodeFactory : IDrawNodeFactory
    {
        public IFillDrawNode CreateFillDrawNode(double x, double y, double width, double height, BaseColor color)
        {
            return new FillDrawNode(x, y, width, height, color);
        }

        public IGridDrawNode CreateGridDrawNode(IGrid grid, double x, double y, double width, double height)
        {
            return new GridDrawNode(grid, x, y, width, height);
        }

        public IPointDrawNode CreatePointDrawNode(PointShape point, ShapeStyle pointStyle, double pointSize)
        {
            return new PointDrawNode(point, pointStyle, pointSize);
        }

        public ILineDrawNode CreateLineDrawNode(LineShape line, ShapeStyle style)
        {
            return new LineDrawNode(line, style);
        }

        public IRectangleDrawNode CreateRectangleDrawNode(RectangleShape rectangle, ShapeStyle style)
        {
            return new RectangleDrawNode(rectangle, style);
        }

        public IEllipseDrawNode CreateEllipseDrawNode(EllipseShape ellipse, ShapeStyle style)
        {
            return new EllipseDrawNode(ellipse, style);
        }

        public IArcDrawNode CreateArcDrawNode(ArcShape arc, ShapeStyle style)
        {
            return new ArcDrawNode(arc, style);
        }

        public ICubicBezierDrawNode CreateCubicBezierDrawNode(CubicBezierShape cubicBezier, ShapeStyle style)
        {
            return new CubicBezierDrawNode(cubicBezier, style);
        }

        public IQuadraticBezierDrawNode CreateQuadraticBezierDrawNode(QuadraticBezierShape quadraticBezier, ShapeStyle style)
        {
            return new QuadraticBezierDrawNode(quadraticBezier, style);
        }

        public ITextDrawNode CreateTextDrawNode(TextShape text, ShapeStyle style)
        {
            return new TextDrawNode(text, style);
        }

        public IImageDrawNode CreateImageDrawNode(ImageShape image, ShapeStyle style, IImageCache imageCache, ICache<string, IDisposable> bitmapCache)
        {
            return new ImageDrawNode(image, style, imageCache, bitmapCache);
        }

        public IPathDrawNode CreatePathDrawNode(PathShape path, ShapeStyle style)
        {
            return new PathDrawNode(path, style);
        }
    }
}
