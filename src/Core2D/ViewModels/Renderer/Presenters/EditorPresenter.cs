using Core2D.Containers;

namespace Core2D.Renderer.Presenters
{
    public class EditorPresenter : IContainerPresenter
    {
        public void Render(object dc, IShapeRenderer renderer, PageContainerViewModel containerViewModel, double dx, double dy)
        {
            renderer.DrawPage(dc, containerViewModel);

            if (containerViewModel.WorkingLayer != null)
            {
                renderer.DrawLayer(dc, containerViewModel.WorkingLayer);
            }

            if (containerViewModel.HelperLayer != null)
            {
                renderer.DrawLayer(dc, containerViewModel.HelperLayer);
            }
        }
    }
}
