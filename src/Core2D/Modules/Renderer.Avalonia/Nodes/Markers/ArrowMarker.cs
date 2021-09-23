#nullable enable
using A = Avalonia;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes.Markers
{
    internal class ArrowMarker : MarkerBase
    {
        public A.Point P11;
        public A.Point P21;
        public A.Point P12;
        public A.Point P22;

        public override void Draw(object dc)
        {
            var context = dc as AP.IDrawingContextImpl;

            if (ShapeViewModel.IsStroked)
            {
                context.DrawLine(Pen, P11, P21);
                context.DrawLine(Pen, P12, P22);
            }
        }
    }
}
