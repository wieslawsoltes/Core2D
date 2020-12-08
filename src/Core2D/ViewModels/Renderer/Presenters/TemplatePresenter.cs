using Core2D.Containers;

namespace Core2D.Renderer.Presenters
{
    public class TemplatePresenter : IContainerPresenter
    {
        public void Render(object dc, IShapeRenderer renderer, PageContainer container, double dx, double dy)
        {
            renderer.Fill(dc, dx, dy, container.Template.Width, container.Template.Height, container.Template.Background);

            renderer.Grid(dc, container.Template, 0, 0, container.Template.Width, container.Template.Height);

            if (container.Template != null)
            {
                renderer.DrawPage(dc, container.Template);
            }
        }
    }
}
