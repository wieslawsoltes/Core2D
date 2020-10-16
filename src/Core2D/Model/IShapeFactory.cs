using Core2D.Path;
using Core2D.Shapes;

namespace Core2D
{
    public interface IShapeFactory
    {
        PointShape Point(double x, double y, bool isStandalone);

        LineShape Line(double x1, double y1, double x2, double y2, bool isStroked);

        LineShape Line(PointShape start, PointShape end, bool isStroked);

        ArcShape Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled);

        ArcShape Arc(PointShape point1, PointShape point2, PointShape point3, PointShape point4, bool isStroked, bool isFilled);

        CubicBezierShape CubicBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled);

        CubicBezierShape CubicBezier(PointShape point1, PointShape point2, PointShape point3, PointShape point4, bool isStroked, bool isFilled);

        QuadraticBezierShape QuadraticBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled);

        QuadraticBezierShape QuadraticBezier(PointShape point1, PointShape point2, PointShape point3, bool isStroked, bool isFilled);

        PathGeometry Geometry(FillRule fillRule);

        PathShape Path(PathGeometry geometry, bool isStroked, bool isFilled);

        RectangleShape Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        RectangleShape Rectangle(PointShape topLeft, PointShape bottomRight, bool isStroked, bool isFilled, string text);

        EllipseShape Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        EllipseShape Ellipse(PointShape topLeft, PointShape bottomRight, bool isStroked, bool isFilled, string text);

        TextShape Text(double x1, double y1, double x2, double y2, string text, bool isStroked);

        TextShape Text(PointShape topLeft, PointShape bottomRight, string text, bool isStroked);

        ImageShape Image(string path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        ImageShape Image(string path, PointShape topLeft, PointShape bottomRight, bool isStroked, bool isFilled, string text);
    }
}
