using Core2D.Containers;

namespace Core2D.Renderer.Presenters
{
    public class TemplatePresenter : IContainerPresenter
    {
        public void Render(object dc, IShapeRenderer renderer, PageContainer container, double dx, double dy)
        {
            renderer.Fill(dc, dx, dy, container.Width, container.Height, container.Background);

            renderer.Grid(dc, container, 0, 0, container.Width, container.Height);

            if (container.Template != null)
            {
                renderer.DrawPage(dc, container.Template);
            }
        }
    }
}
