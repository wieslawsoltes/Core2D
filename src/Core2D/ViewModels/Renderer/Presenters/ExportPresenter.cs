using Core2D.Containers;

namespace Core2D.Renderer.Presenters
{
    public class ExportPresenter : IContainerPresenter
    {
        public void Render(object dc, IShapeRenderer renderer, PageContainerViewModel containerViewModel, double dx, double dy)
        {
            var flags = renderer.StateViewModel.DrawShapeState;

            renderer.StateViewModel.DrawShapeState = ShapeStateFlags.Printable;

            renderer.Fill(dc, dx, dy, containerViewModel.Template.Width, containerViewModel.Template.Height, containerViewModel.Template.Background);

            if (containerViewModel.Template != null)
            {
                renderer.DrawPage(dc, containerViewModel.Template);
            }

            renderer.DrawPage(dc, containerViewModel);

            renderer.StateViewModel.DrawShapeState = flags;
        }
    }
}
