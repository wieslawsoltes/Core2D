#nullable enable
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes.Markers;

internal class EllipseMarker : MarkerBase
{
    public SKRect Rect { get; set; }

    public override void Draw(object? dc)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        var count = canvas.Save();
        canvas.SetMatrix(MatrixHelper.Multiply(Rotation, canvas.TotalMatrix));

        if (ShapeViewModel.IsFilled)
        {
            canvas.DrawOval(Rect, Brush);
        }

        if (ShapeViewModel.IsStroked)
        {
            canvas.DrawOval(Rect, Pen);
        }

        canvas.RestoreToCount(count);
    }
}
