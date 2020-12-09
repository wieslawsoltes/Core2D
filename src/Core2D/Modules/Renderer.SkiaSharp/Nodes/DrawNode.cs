using Core2D.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal abstract class DrawNode : IDrawNode
    {
        public ShapeStyleViewModel StyleViewModel { get; set; }
        public bool ScaleThickness { get; set; }
        public bool ScaleSize { get; set; }
        public SKPaint Fill { get; set; }
        public SKPaint Stroke { get; set; }
        public SKPoint Center { get; set; }

        public DrawNode()
        {
        }

        public abstract void UpdateGeometry();

        public virtual void UpdateStyle()
        {
            Fill = SkiaSharpDrawUtil.ToSKPaintBrush(StyleViewModel.Fill.ColorViewModel);
            Stroke = SkiaSharpDrawUtil.ToSKPaintPen(StyleViewModel, StyleViewModel.Stroke.Thickness);
        }

        public virtual void Draw(object dc, double zoom)
        {
            var scale = ScaleSize ? 1.0 / zoom : 1.0;
            var translateX = 0.0 - (Center.X * scale) + Center.X;
            var translateY = 0.0 - (Center.Y * scale) + Center.Y;

            double thickness = StyleViewModel.Stroke.Thickness;

            if (ScaleThickness)
            {
                thickness /= zoom;
            }

            if (scale != 1.0)
            {
                thickness /= scale;
            }

            if (Stroke.StrokeWidth != thickness)
            {
                Stroke.StrokeWidth = (float)thickness;
            }

            var canvas = dc as SKCanvas;

            int count = int.MinValue;

            if (scale != 1.0)
            {
                count = canvas.Save();
                canvas.Translate((float)translateX, (float)translateY);
                canvas.Scale((float)scale, (float)scale);
            }

            OnDraw(dc, zoom);

            if (scale != 1.0)
            {
                canvas.RestoreToCount(count);
            }
        }

        public abstract void OnDraw(object dc, double zoom);

        public virtual void Dispose()
        {
        }
    }
}
