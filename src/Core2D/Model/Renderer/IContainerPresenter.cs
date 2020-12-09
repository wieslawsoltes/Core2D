using Core2D.Containers;

namespace Core2D.Renderer
{
    public interface IContainerPresenter
    {
        void Render(object dc, IShapeRenderer renderer, PageContainerViewModel containerViewModel, double dx, double dy);
    }
}
