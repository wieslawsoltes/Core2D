#nullable enable
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes.Markers;

internal class ArrowMarker : MarkerBase
{
    public SKPoint P11;
    public SKPoint P21;
    public SKPoint P12;
    public SKPoint P22;

    public override void Draw(object? dc)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        if (ShapeViewModel is { } && ShapeViewModel.IsStroked)
        {
            canvas.DrawLine(P11, P21, Pen);
            canvas.DrawLine(P12, P22, Pen);
        }
    }
}
