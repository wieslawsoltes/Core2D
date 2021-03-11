#nullable disable
using Core2D.Modules.Renderer.Media;
using A = Avalonia;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Nodes.Markers
{
    internal class RectangleMarker : MarkerBase
    {
        public A.Rect Rect { get; set; }

        public override void Draw(object dc)
        {
            var context = dc as AP.IDrawingContextImpl;
            using var rotationDisposable = context.PushPreTransform(Rotation);

            if (ShapeViewModel.IsFilled)
            {
                context.DrawRectangle(Brush, null, Rect);
            }

            if (ShapeViewModel.IsStroked)
            {
                context.DrawRectangle(null, Pen, Rect);
            }
        }
    }
}
