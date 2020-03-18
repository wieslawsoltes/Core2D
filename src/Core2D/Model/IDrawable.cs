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
        IShapeStyle Style { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether shape is stroked.
        /// </summary>
        bool IsStroked { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether shape is filled.
        /// </summary>
        bool IsFilled { get; set; }

        /// <summary>
        /// Get or sets shape matrix transform.
        /// </summary>
        IMatrixObject Transform { get; set; }

        /// <summary>
        /// Begins matrix transform.
        /// </summary>
        /// <param name="dc">The generic drawing context object.</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <returns>The previous transform state.</returns>
        object BeginTransform(object dc, IShapeRenderer renderer);

        /// <summary>
        /// Ends matrix transform.
        /// </summary>
        /// <param name="dc">The generic drawing context object.</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="state">The previous transform state.</param>
        void EndTransform(object dc, IShapeRenderer renderer, object state);

        /// <summary>
        /// Draws shape using current <see cref="IShapeRenderer"/>.
        /// </summary>
        /// <param name="dc">The generic drawing context object.</param>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="dx">The X axis draw position offset.</param>
        /// <param name="dy">The Y axis draw position offset.</param>
        void Draw(object dc, IShapeRenderer renderer, double dx, double dy);

        /// <summary>
        /// Invalidates shape renderer cache.
        /// </summary>
        /// <param name="renderer">The generic renderer object used to draw shape.</param>
        /// <param name="dx">The X axis draw position offset.</param>
        /// <param name="dy">The Y axis draw position offset.</param>
        /// <returns>Returns true if shape was invalidated; otherwise, returns false.</returns>
        bool Invalidate(IShapeRenderer renderer, double dx, double dy);
    }
}
