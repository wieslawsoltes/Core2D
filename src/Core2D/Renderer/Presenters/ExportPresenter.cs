using Core2D.Containers;

namespace Core2D.Renderer.Presenters
{
    /// <summary>
    /// Export container presenter.
    /// </summary>
    public class ExportPresenter : IContainerPresenter
    {
        /// <inheritdoc/>
        public void Render(object dc, IShapeRenderer renderer, IPageContainer container, double dx, double dy)
        {
            var flags = renderer.State.DrawShapeState.Flags;

            renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;

            renderer.Fill(dc, dx, dy, container.Width, container.Height, container.Background);

            if (container.Template != null)
            {
                renderer.Draw(dc, container.Template, dx, dy);
            }

            renderer.Draw(dc, container, dx, dy);

            renderer.State.DrawShapeState.Flags = flags;
        }
    }
}
