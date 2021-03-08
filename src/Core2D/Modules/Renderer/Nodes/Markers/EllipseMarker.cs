#nullable disable
using Core2D.Modules.Renderer.Media;
using AP = Avalonia.Platform;
using AM = Avalonia.Media;

namespace Core2D.Modules.Renderer.Nodes.Markers
{
    internal class EllipseMarker : MarkerBase
    {
        public AM.EllipseGeometry EllipseGeometry { get; set; }

        public override void Draw(object dc)
        {
#if CUSTOM_DRAW
            var context = dc as AP.IDrawingContextImpl;
            using var rotationDisposable = context.PushPreTransform(Rotation);
            context.DrawGeometry(ShapeViewModel.IsFilled ? Brush : null, ShapeViewModel.IsStroked ? Pen : null, EllipseGeometry.PlatformImpl);
#else
            var context = dc as AM.DrawingContext;
            using var rotationDisposable = context.PushPreTransform(Rotation);
            context.DrawGeometry(ShapeViewModel.IsFilled ? Brush : null, ShapeViewModel.IsStroked ? Pen : null, EllipseGeometry);
#endif
        }
    }
}
