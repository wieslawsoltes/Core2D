using Core2D.Containers;

namespace Core2D.Renderer.Presenters
{
    /// <summary>
    /// Editor container presenter.
    /// </summary>
    public class EditorPresenter : IContainerPresenter
    {
        /// <inheritdoc/>
        public void Render(object dc, IShapeRenderer renderer, PageContainer container, double dx, double dy)
        {
            renderer.DrawPage(dc, container);

            if (container.WorkingLayer != null)
            {
                renderer.DrawLayer(dc, container.WorkingLayer);
            }

            if (container.HelperLayer != null)
            {
                renderer.DrawLayer(dc, container.HelperLayer);
            }
        }
    }
}
