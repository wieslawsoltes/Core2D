using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;

namespace Core2D.ViewModels.Renderer.Presenters
{
    public partial class TemplatePresenter : IContainerPresenter
    {
        public void Render(object dc, IShapeRenderer renderer, BaseContainerViewModel container, double dx, double dy)
        {
            if (container is PageContainerViewModel page && page.Template != null)
            {
                renderer.Fill(dc, dx, dy, page.Template.Width, page.Template.Height, page.Template.Background);
                renderer.Grid(dc, page.Template, 0, 0, page.Template.Width, page.Template.Height);
                renderer.DrawContainer(dc, page.Template);
            }
        }
    }
}
