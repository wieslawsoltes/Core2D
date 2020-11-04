using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    public interface IShapeRenderer
    {
        ShapeRendererState State { get; set; }

        void ClearCache();

        void Fill(object dc, double x, double y, double width, double height, BaseColor color);

        void Grid(object dc, IGrid grid, double x, double y, double width, double height);

        void DrawPage(object dc, PageContainer container);

        void DrawLayer(object dc, LayerContainer layer);

        void DrawPoint(object dc, PointShape point);

        void DrawLine(object dc, LineShape line);

        void DrawRectangle(object dc, RectangleShape rectangle);

        void DrawEllipse(object dc, EllipseShape ellipse);

        void DrawArc(object dc, ArcShape arc);

        void DrawCubicBezier(object dc, CubicBezierShape cubicBezier);

        void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier);

        void DrawText(object dc, TextShape text);

        void DrawImage(object dc, ImageShape image);

        void DrawPath(object dc, PathShape path);
    }
}
