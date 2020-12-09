using Core2D.Containers;

namespace Core2D.Renderer.Presenters
{
    public class TemplatePresenter : IContainerPresenter
    {
        public void Render(object dc, IShapeRenderer renderer, PageContainerViewModel containerViewModel, double dx, double dy)
        {
            renderer.Fill(dc, dx, dy, containerViewModel.Template.Width, containerViewModel.Template.Height, containerViewModel.Template.Background);

            renderer.Grid(dc, containerViewModel.Template, 0, 0, containerViewModel.Template.Width, containerViewModel.Template.Height);

            if (containerViewModel.Template != null)
            {
                renderer.DrawPage(dc, containerViewModel.Template);
            }
        }
    }
}
