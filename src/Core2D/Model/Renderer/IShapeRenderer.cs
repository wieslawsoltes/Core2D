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
        /// Invalidates style cache.
        /// </summary>
        /// <param name="style">The style to invalidate.</param>
        void InvalidateCache(IShapeStyle style);

        /// <summary>
        /// Invalidates matrix cache.
        /// </summary>
        /// <param name="matrix">The matrix to invalidate.</param>
        void InvalidateCache(IMatrixObject matrix);

        /// <summary>
        /// Invalidates shape cache.
        /// </summary>
        /// <param name="shape">The shape to invalidate.</param>
        /// <param name="style">The style to invalidate.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void InvalidateCache(IBaseShape shape, IShapeStyle style, double dx, double dy);

        /// <summary>
        /// Clears renderer cache.
        /// </summary>
        /// <param name="isZooming">The flag indicating zooming state.</param>
        void ClearCache(bool isZooming);

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
        /// Push matrix.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="matrix">The matrix to push.</param>
        /// <returns>The previous matrix state.</returns>
        object PushMatrix(object dc, IMatrixObject matrix);

        /// <summary>
        /// Pop matrix.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="state">The previous matrix state.</param>
        void PopMatrix(object dc, object state);

        /// <summary>
        /// Draws a <see cref="IPageContainer"/> using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="container">The <see cref="IPageContainer"/> object.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void Draw(object dc, IPageContainer container, double dx, double dy);

        /// <summary>
        /// Draws a <see cref="ILayerContainer"/> using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="layer">The <see cref="ILayerContainer"/> object.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void Draw(object dc, ILayerContainer layer, double dx, double dy);

        /// <summary>
        /// Draws a <see cref="ILineShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="line">The <see cref="ILineShape"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void Draw(object dc, ILineShape line, double dx, double dy);

        /// <summary>
        /// Draws a <see cref="IRectangleShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="rectangle">The <see cref="IRectangleShape"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void Draw(object dc, IRectangleShape rectangle, double dx, double dy);

        /// <summary>
        /// Draws a <see cref="IEllipseShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="ellipse">The <see cref="IEllipseShape"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void Draw(object dc, IEllipseShape ellipse, double dx, double dy);

        /// <summary>
        /// Draws a <see cref="IArcShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="arc">The <see cref="IArcShape"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void Draw(object dc, IArcShape arc, double dx, double dy);

        /// <summary>
        /// Draws a <see cref="ICubicBezierShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="cubicBezier">The <see cref="ICubicBezierShape"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void Draw(object dc, ICubicBezierShape cubicBezier, double dx, double dy);

        /// <summary>
        /// Draws a <see cref="IQuadraticBezierShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="quadraticBezier">The <see cref="IQuadraticBezierShape"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void Draw(object dc, IQuadraticBezierShape quadraticBezier, double dx, double dy);

        /// <summary>
        /// Draws a <see cref="ITextShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="text">The <see cref="ITextShape"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void Draw(object dc, ITextShape text, double dx, double dy);

        /// <summary>
        /// Draws a <see cref="IImageShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="image">The <see cref="IImageShape"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void Draw(object dc, IImageShape image, double dx, double dy);

        /// <summary>
        /// Draws a <see cref="IPathShape"/> shape using drawing context.
        /// </summary>
        /// <param name="dc">The native drawing context.</param>
        /// <param name="path">The <see cref="IPathShape"/> shape.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void Draw(object dc, IPathShape path, double dx, double dy);
    }
}
