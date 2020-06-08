using System;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    public interface IDrawNodeFactory
    {
        IFillDrawNode CreateFillDrawNode(double x, double y, double width, double height, IColor color);
        IPointDrawNode CreatePointDrawNode(IPointShape point, IShapeStyle pointStyle, double pointSize);
        ILineDrawNode CreateLineDrawNode(ILineShape line, IShapeStyle style);
        IRectangleDrawNode CreateRectangleDrawNode(IRectangleShape rectangle, IShapeStyle style);
        IEllipseDrawNode CreateEllipseDrawNode(IEllipseShape ellipse, IShapeStyle style);
        IArcDrawNode CreateArcDrawNode(IArcShape arc, IShapeStyle style);
        ICubicBezierDrawNode CreateCubicBezierDrawNode(ICubicBezierShape cubicBezier, IShapeStyle style);
        IQuadraticBezierDrawNode CreateQuadraticBezierDrawNode(IQuadraticBezierShape quadraticBezier, IShapeStyle style);
        ITextDrawNode CreateTextDrawNode(ITextShape text, IShapeStyle style);
        IImageDrawNode CreateImageDrawNode(IImageShape image, IShapeStyle style, IImageCache imageCache, ICache<string, IDisposable> bitmapCache);
        IPathDrawNode CreatePathDrawNode(IPathShape path, IShapeStyle style);
    }
}
