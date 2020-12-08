using Core2D.Containers;

namespace Core2D.Renderer.Presenters
{
    public class ExportPresenter : IContainerPresenter
    {
        public void Render(object dc, IShapeRenderer renderer, PageContainer container, double dx, double dy)
        {
            var flags = renderer.State.DrawShapeState;

            renderer.State.DrawShapeState = ShapeStateFlags.Printable;

            renderer.Fill(dc, dx, dy, container.Template.Width, container.Template.Height, container.Template.Background);

            if (container.Template != null)
            {
                renderer.DrawPage(dc, container.Template);
            }

            renderer.DrawPage(dc, container);

            renderer.State.DrawShapeState = flags;
        }
    }
}
