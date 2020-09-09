using Core2D.Renderer;
using Core2D.Style;

namespace Core2D
{
    /// <summary>
    /// Defines drawable shape contract.
    /// </summary>
    public interface IDrawable
    {
        /// <summary>
        /// Get or sets shape drawing style.
        /// </summary>
        IShapeStyle? Style { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether shape is stroked.
        /// </summary>
        bool IsStroked { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether shape is filled.
        /// </summary>
        bool IsFilled { get; set; }

        /// <summary>
        /// Draws shape using current <see cref="IShapeRenderer"/>.
        /// </summary>
        /// <param name="dc">The generic drawing context object.</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        void DrawShape(object dc, IShapeRenderer renderer);

        /// <summary>
        /// Draws points using current <see cref="IShapeRenderer"/>.
        /// </summary>
        /// <param name="dc">The generic drawing context object.</param>
        /// <param name="renderer">The generic renderer object used to draw points.</param>
        void DrawPoints(object dc, IShapeRenderer renderer);

        /// <summary>
        /// Invalidates shape renderer cache.
        /// </summary>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <returns>Returns true if shape was invalidated; otherwise, returns false.</returns>
        bool Invalidate(IShapeRenderer renderer);
    }
}
