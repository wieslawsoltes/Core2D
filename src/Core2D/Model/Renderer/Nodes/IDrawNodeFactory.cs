using System;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    public interface IDrawNodeFactory
    {
        IFillDrawNode CreateFillDrawNode(double x, double y, double width, double height, BaseColor color);

        IGridDrawNode CreateGridDrawNode(IGrid grid, double x, double y, double width, double height);

        IPointDrawNode CreatePointDrawNode(PointShape point, ShapeStyle pointStyle, double pointSize);

        ILineDrawNode CreateLineDrawNode(LineShape line, ShapeStyle style);

        IRectangleDrawNode CreateRectangleDrawNode(RectangleShape rectangle, ShapeStyle style);

        IEllipseDrawNode CreateEllipseDrawNode(EllipseShape ellipse, ShapeStyle style);

        IArcDrawNode CreateArcDrawNode(ArcShape arc, ShapeStyle style);

        ICubicBezierDrawNode CreateCubicBezierDrawNode(CubicBezierShape cubicBezier, ShapeStyle style);

        IQuadraticBezierDrawNode CreateQuadraticBezierDrawNode(QuadraticBezierShape quadraticBezier, ShapeStyle style);

        ITextDrawNode CreateTextDrawNode(TextShape text, ShapeStyle style);

        IImageDrawNode CreateImageDrawNode(ImageShape image, ShapeStyle style, IImageCache imageCache, ICache<string, IDisposable> bitmapCache);

        IPathDrawNode CreatePathDrawNode(PathShape path, ShapeStyle style);
    }
}
