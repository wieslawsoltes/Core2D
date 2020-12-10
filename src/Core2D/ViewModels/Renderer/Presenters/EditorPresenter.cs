using Core2D.Containers;

namespace Core2D.Renderer.Presenters
{
    public class EditorPresenter : IContainerPresenter
    {
        public void Render(object dc, IShapeRenderer renderer, PageContainerViewModel container, double dx, double dy)
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
