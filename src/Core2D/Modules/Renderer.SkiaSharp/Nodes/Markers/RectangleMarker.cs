using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class RectangleMarker : Marker
    {
        public SKRect Rect { get; set; }

        public override void Draw(object dc)
        {
            var canvas = dc as SKCanvas;

            var count = canvas.Save();
            canvas.SetMatrix(MatrixHelper.Multiply(Rotation, canvas.TotalMatrix));

            if (Shape.IsFilled)
            {
                canvas.DrawRect(Rect, Brush);
            }

            if (Shape.IsStroked)
            {
                canvas.DrawRect(Rect, Pen);
            }

            canvas.RestoreToCount(count);
        }
    }
}
