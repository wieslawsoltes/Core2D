using System;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer.SkiaSharp
{
    internal class SkiaSharpDrawNodeFactory : IDrawNodeFactory
    {
        public IFillDrawNode CreateFillDrawNode(double x, double y, double width, double height, IColor color)
        {
            return new FillDrawNode(x, y, width, height, color);
        }

        public IGridDrawNode CreateGridDrawNode(IGrid grid, double x, double y, double width, double height)
        {
            return new GridDrawNode(grid, x, y, width, height);
        }

        public IPointDrawNode CreatePointDrawNode(IPointShape point, IShapeStyle pointStyle, double pointSize)
        {
            return new PointDrawNode(point, pointStyle, pointSize);
        }

        public ILineDrawNode CreateLineDrawNode(ILineShape line, IShapeStyle style)
        {
            return new LineDrawNode(line, style);
        }

        public IRectangleDrawNode CreateRectangleDrawNode(IRectangleShape rectangle, IShapeStyle style)
        {
            return new RectangleDrawNode(rectangle, style);
        }

        public IEllipseDrawNode CreateEllipseDrawNode(IEllipseShape ellipse, IShapeStyle style)
        {
            return new EllipseDrawNode(ellipse, style);
        }

        public IArcDrawNode CreateArcDrawNode(IArcShape arc, IShapeStyle style)
        {
            return new ArcDrawNode(arc,style);
        }

        public ICubicBezierDrawNode CreateCubicBezierDrawNode(ICubicBezierShape cubicBezier, IShapeStyle style)
        {
            return new CubicBezierDrawNode(cubicBezier, style);
        }

        public IQuadraticBezierDrawNode CreateQuadraticBezierDrawNode(IQuadraticBezierShape quadraticBezier, IShapeStyle style)
        {
            return new QuadraticBezierDrawNode(quadraticBezier, style);
        }

        public ITextDrawNode CreateTextDrawNode(ITextShape text, IShapeStyle style)
        {
            return new TextDrawNode(text, style);
        }

        public IImageDrawNode CreateImageDrawNode(IImageShape image, IShapeStyle style, IImageCache imageCache, ICache<string, IDisposable> bitmapCache)
        {
            return new ImageDrawNode(image, style, imageCache, bitmapCache);
        }

        public IPathDrawNode CreatePathDrawNode(IPathShape path, IShapeStyle style)
        {
            return new PathDrawNode(path, style);
        }
    }
}
