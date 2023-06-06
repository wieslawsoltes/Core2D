#nullable enable
using AP = Avalonia.Platform;
using AM = Avalonia.Media;

namespace Core2D.Modules.Renderer.Avalonia.Nodes.Markers;

internal class EllipseMarker : MarkerBase
{
    public AM.EllipseGeometry? EllipseGeometry { get; set; }

    public override void Draw(object? dc)
    {
        if (dc is not AM.ImmediateDrawingContext context)
        {
            return;
        }

        if (ShapeViewModel is { } && EllipseGeometry is { })
        {
            using var rotationDisposable = context.PushPreTransform(Rotation);
            // TODO: context.DrawGeometry(ShapeViewModel.IsFilled ? Brush : null, ShapeViewModel.IsStroked ? Pen : null, EllipseGeometry);
        }
    }
}
