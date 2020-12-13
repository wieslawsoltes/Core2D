using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;

namespace Core2D.ViewModels.Renderer.Presenters
{
    public partial class EditorPresenter : IContainerPresenter
    {
        public void Render(object dc, IShapeRenderer renderer, BaseContainerViewModel container, double dx, double dy)
        {
            renderer.DrawContainer(dc, container);

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
