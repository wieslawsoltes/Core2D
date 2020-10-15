using Core2D.Containers;

namespace Core2D.Renderer
{
    /// <summary>
    /// Define container presenter contract.
    /// </summary>
    public interface IContainerPresenter
    {
        /// <summary>
        /// Renders a container using provided drawing context.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="container">The container to render.</param>
        /// <param name="dx">The X coordinate offset.</param>
        /// <param name="dy">The Y coordinate offset.</param>
        void Render(object dc, IShapeRenderer renderer, PageContainer container, double dx, double dy);
    }
}
