#nullable disable
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes.Markers
{
    internal class RectangleMarker : MarkerBase
    {
        public SKRect Rect { get; set; }

        public override void Draw(object dc)
        {
            var canvas = dc as SKCanvas;

            var count = canvas.Save();
            canvas.SetMatrix(MatrixHelper.Multiply(Rotation, canvas.TotalMatrix));

            if (ShapeViewModel.IsFilled)
            {
                canvas.DrawRect(Rect, Brush);
            }

            if (ShapeViewModel.IsStroked)
            {
                canvas.DrawRect(Rect, Pen);
            }

            canvas.RestoreToCount(count);
        }
    }
}
