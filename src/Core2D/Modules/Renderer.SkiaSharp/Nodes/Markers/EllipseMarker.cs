using Core2D.Renderer.SkiaSharp;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class EllipseMarker : Marker
    {
        public SKRect Rect { get; set; }

        public override void Draw(object dc)
        {
            var canvas = dc as SKCanvas;

            var count = canvas.Save();
            canvas.SetMatrix(MatrixHelper.Multiply(Rotation, canvas.TotalMatrix));

            if (Shape.IsFilled)
            {
                canvas.DrawOval(Rect, Brush);
            }

            if (Shape.IsStroked)
            {
                canvas.DrawOval(Rect, Pen);
            }

            canvas.RestoreToCount(count);
        }
    }
}
