using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;

namespace Core2D.ViewModels.Renderer.Presenters
{
    public partial class ExportPresenter : IContainerPresenter
    {
        public void Render(object dc, IShapeRenderer renderer, BaseContainerViewModel container, double dx, double dy)
        {
            var flags = renderer.State.DrawShapeState;

            renderer.State.DrawShapeState = ShapeStateFlags.Printable;

            if (container is PageContainerViewModel page && page.Template != null)
            {
                renderer.Fill(dc, dx, dy, page.Template.Width, page.Template.Height, page.Template.Background);
                renderer.DrawContainer(dc, page.Template);
            }

            renderer.DrawContainer(dc, container);
            renderer.State.DrawShapeState = flags;
        }
    }
}
