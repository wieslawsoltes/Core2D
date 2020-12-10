using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    public interface IShapeRenderer
    {
        ShapeRendererStateViewModel State { get; set; }

        void ClearCache();

        void Fill(object dc, double x, double y, double width, double height, BaseColorViewModel color);

        void Grid(object dc, IGrid grid, double x, double y, double width, double height);

        void DrawPage(object dc, PageContainerViewModel container);

        void DrawLayer(object dc, LayerContainerViewModel layer);

        void DrawPoint(object dc, PointShapeViewModel point);

        void DrawLine(object dc, LineShapeViewModel line);

        void DrawRectangle(object dc, RectangleShapeViewModel rectangle);

        void DrawEllipse(object dc, EllipseShapeViewModel ellipse);

        void DrawArc(object dc, ArcShapeViewModelViewModel arc);

        void DrawCubicBezier(object dc, CubicBezierShapeViewModel cubicBezier);

        void DrawQuadraticBezier(object dc, QuadraticBezierShapeViewModel quadraticBezier);

        void DrawText(object dc, TextShapeViewModel text);

        void DrawImage(object dc, ImageShapeViewModel image);

        void DrawPath(object dc, PathShapeViewModel path);
    }
}
