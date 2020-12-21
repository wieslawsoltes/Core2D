#nullable disable
namespace Core2D.Modules.Renderer.Nodes.Markers
{
    internal class ArrowMarker : MarkerBase
    {
        public Avalonia.Point P11;
        public Avalonia.Point P21;
        public Avalonia.Point P12;
        public Avalonia.Point P22;

        public override void Draw(object dc)
        {
            var context = dc as Avalonia.Media.DrawingContext;

            if (ShapeViewModel.IsStroked)
            {
                context.DrawLine(Pen, P11, P21);
                context.DrawLine(Pen, P12, P22);
            }
        }
    }
}
