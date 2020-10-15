using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    /// <summary>
    /// Defines shape renderer contract.
    /// </summary>
    public interface IShapeRenderer
    {
        /// <summary>
        /// Gets or sets renderer state.
        /// </summary>
        ShapeRendererState State { get; set; }

        /// <summary>
        /// Clears renderer cache.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Fills rectangle with specified color using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="x">The X coordinate of rectangle origin point.</param>
        /// <param name="y">The Y coordinate of rectangle origin point.</param>
        /// <param name="width">The width of rectangle.</param>
        /// <param name="height">The height of rectangle.</param>
        /// <param name="color">The fill color.</param>
        void Fill(object dc, double x, double y, double width, double height, BaseColor color);

        /// <summary>
        /// Draws a <see cref="IGrid"/> using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="grid">The <see cref="IGrid"/> object.</param>
        /// <param name="x">The X coordinate of grid origin point.</param>
        /// <param name="y">The Y coordinate of grid origin point.</param>
        /// <param name="width">The width of grid.</param>
        /// <param name="height">The height of grid.</param>
        void Grid(object dc, IGrid grid, double x, double y, double width, double height);

        /// <summary>
        /// Draws a <see cref="PageContainer"/> using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="container">The <see cref="PageContainer"/> object.</param>
        void DrawPage(object dc, PageContainer container);

        /// <summary>
        /// Draws a <see cref="LayerContainer"/> using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="layer">The <see cref="LayerContainer"/> object.</param>
        void DrawLayer(object dc, LayerContainer layer);

        /// <summary>
        /// Draws a <see cref="PointShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="point">The <see cref="PointShape"/> shape.</param>
        void DrawPoint(object dc, PointShape point);

        /// <summary>
        /// Draws a <see cref="LineShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="line">The <see cref="LineShape"/> shape.</param>
        void DrawLine(object dc, LineShape line);

        /// <summary>
        /// Draws a <see cref="RectangleShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="rectangle">The <see cref="RectangleShape"/> shape.</param>
        void DrawRectangle(object dc, RectangleShape rectangle);

        /// <summary>
        /// Draws a <see cref="EllipseShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="ellipse">The <see cref="EllipseShape"/> shape.</param>
        void DrawEllipse(object dc, EllipseShape ellipse);

        /// <summary>
        /// Draws a <see cref="ArcShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="arc">The <see cref="ArcShape"/> shape.</param>
        void DrawArc(object dc, ArcShape arc);

        /// <summary>
        /// Draws a <see cref="CubicBezierShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="cubicBezier">The <see cref="CubicBezierShape"/> shape.</param>
        void DrawCubicBezier(object dc, CubicBezierShape cubicBezier);

        /// <summary>
        /// Draws a <see cref="QuadraticBezierShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="quadraticBezier">The <see cref="QuadraticBezierShape"/> shape.</param>
        void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier);

        /// <summary>
        /// Draws a <see cref="TextShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="text">The <see cref="TextShape"/> shape.</param>
        void DrawText(object dc, TextShape text);

        /// <summary>
        /// Draws a <see cref="ImageShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="image">The <see cref="ImageShape"/> shape.</param>
        void DrawImage(object dc, ImageShape image);

        /// <summary>
        /// Draws a <see cref="PathShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="path">The <see cref="PathShape"/> shape.</param>
        void DrawPath(object dc, PathShape path);
    }
}
