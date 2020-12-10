using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;

namespace Core2D.ViewModels.Renderer.Presenters
{
    public partial class TemplatePresenter : IContainerPresenter
    {
        public void Render(object dc, IShapeRenderer renderer, PageContainerViewModel container, double dx, double dy)
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
