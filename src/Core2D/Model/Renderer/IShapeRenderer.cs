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
        IShapeRendererState State { get; set; }

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
        void Fill(object dc, double x, double y, double width, double height, IColor color);

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
        /// Draws a <see cref="IPageContainer"/> using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="container">The <see cref="IPageContainer"/> object.</param>
        void DrawPage(object dc, IPageContainer container);

        /// <summary>
        /// Draws a <see cref="ILayerContainer"/> using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="layer">The <see cref="ILayerContainer"/> object.</param>
        void DrawLayer(object dc, ILayerContainer layer);

        /// <summary>
        /// Draws a <see cref="IPointShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="point">The <see cref="IPointShape"/> shape.</param>
        void DrawPoint(object dc, IPointShape point);

        /// <summary>
        /// Draws a <see cref="ILineShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="line">The <see cref="ILineShape"/> shape.</param>
        void DrawLine(object dc, ILineShape line);

        /// <summary>
        /// Draws a <see cref="IRectangleShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="rectangle">The <see cref="IRectangleShape"/> shape.</param>
        void DrawRectangle(object dc, IRectangleShape rectangle);

        /// <summary>
        /// Draws a <see cref="IEllipseShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="ellipse">The <see cref="IEllipseShape"/> shape.</param>
        void DrawEllipse(object dc, IEllipseShape ellipse);

        /// <summary>
        /// Draws a <see cref="IArcShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="arc">The <see cref="IArcShape"/> shape.</param>
        void DrawArc(object dc, IArcShape arc);

        /// <summary>
        /// Draws a <see cref="ICubicBezierShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="cubicBezier">The <see cref="ICubicBezierShape"/> shape.</param>
        void DrawCubicBezier(object dc, ICubicBezierShape cubicBezier);

        /// <summary>
        /// Draws a <see cref="IQuadraticBezierShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="quadraticBezier">The <see cref="IQuadraticBezierShape"/> shape.</param>
        void DrawQuadraticBezier(object dc, IQuadraticBezierShape quadraticBezier);

        /// <summary>
        /// Draws a <see cref="ITextShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="text">The <see cref="ITextShape"/> shape.</param>
        void DrawText(object dc, ITextShape text);

        /// <summary>
        /// Draws a <see cref="IImageShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="image">The <see cref="IImageShape"/> shape.</param>
        void DrawImage(object dc, IImageShape image);

        /// <summary>
        /// Draws a <see cref="IPathShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="path">The <see cref="IPathShape"/> shape.</param>
        void DrawPath(object dc, IPathShape path);
    }
}
