using Core2D.Model.Path;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Shapes;

namespace Core2D.Model
{
    public interface IShapeFactory
    {
        PointShapeViewModel Point(double x, double y, bool isStandalone);

        LineShapeViewModel Line(double x1, double y1, double x2, double y2, bool isStroked);

        LineShapeViewModel Line(PointShapeViewModel start, PointShapeViewModel end, bool isStroked);

        ArcShapeViewModelViewModel Arc(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled);

        ArcShapeViewModelViewModel Arc(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3, PointShapeViewModel point4, bool isStroked, bool isFilled);

        CubicBezierShapeViewModel CubicBezier(double x1, double y1, double x2, double y2, double x3, double y3, double x4, double y4, bool isStroked, bool isFilled);

        CubicBezierShapeViewModel CubicBezier(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3, PointShapeViewModel point4, bool isStroked, bool isFilled);

        QuadraticBezierShapeViewModel QuadraticBezier(double x1, double y1, double x2, double y2, double x3, double y3, bool isStroked, bool isFilled);

        QuadraticBezierShapeViewModel QuadraticBezier(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3, bool isStroked, bool isFilled);

        PathGeometryViewModel Geometry(FillRule fillRule);

        PathShapeViewModel Path(PathGeometryViewModel geometry, bool isStroked, bool isFilled);

        RectangleShapeViewModel Rectangle(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        RectangleShapeViewModel Rectangle(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, bool isStroked, bool isFilled, string text);

        EllipseShapeViewModel Ellipse(double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        EllipseShapeViewModel Ellipse(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, bool isStroked, bool isFilled, string text);

        TextShapeViewModel Text(double x1, double y1, double x2, double y2, string text, bool isStroked);

        TextShapeViewModel Text(PointShapeViewModel topLeft, PointShapeViewModel bottomRight, string text, bool isStroked);

        ImageShapeViewModel Image(string path, double x1, double y1, double x2, double y2, bool isStroked, bool isFilled, string text);

        ImageShapeViewModel Image(string path, PointShapeViewModel topLeft, PointShapeViewModel bottomRight, bool isStroked, bool isFilled, string text);
    }
}
