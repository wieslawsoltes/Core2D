using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Renderer;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Model.Renderer
{
    public interface IShapeRenderer
    {
        ShapeRendererStateViewModel State { get; set; }

        void ClearCache();

        void Fill(object dc, double x, double y, double width, double height, BaseColorViewModel color);

        void Grid(object dc, IGrid grid, double x, double y, double width, double height);

        void DrawPoint(object dc, PointShapeViewModel point, ShapeStyleViewModel style);

        void DrawLine(object dc, LineShapeViewModel line, ShapeStyleViewModel style);

        void DrawRectangle(object dc, RectangleShapeViewModel rectangle, ShapeStyleViewModel style);

        void DrawEllipse(object dc, EllipseShapeViewModel ellipse, ShapeStyleViewModel style);

        void DrawArc(object dc, ArcShapeViewModel arc, ShapeStyleViewModel style);

        void DrawCubicBezier(object dc, CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel style);

        void DrawQuadraticBezier(object dc, QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel style);

        void DrawText(object dc, TextShapeViewModel text, ShapeStyleViewModel style);

        void DrawImage(object dc, ImageShapeViewModel image, ShapeStyleViewModel style);

        void DrawPath(object dc, PathShapeViewModel path, ShapeStyleViewModel style);
    }
}
