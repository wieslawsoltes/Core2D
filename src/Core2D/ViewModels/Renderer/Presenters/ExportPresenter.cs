using Core2D.Containers;

namespace Core2D.Renderer.Presenters
{
    public class ExportPresenter : IContainerPresenter
    {
        public void Render(object dc, IShapeRenderer renderer, PageContainer container, double dx, double dy)
        {
            var flags = renderer.State.DrawShapeState.Flags;

            renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;

            renderer.Fill(dc, dx, dy, container.Width, container.Height, container.Background);

            if (container.Template != null)
            {
                renderer.DrawPage(dc, container.Template);
            }

            renderer.DrawPage(dc, container);

            renderer.State.DrawShapeState.Flags = flags;
        }
    }
}
