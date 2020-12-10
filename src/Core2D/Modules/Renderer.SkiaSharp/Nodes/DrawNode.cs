using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal abstract class DrawNode : IDrawNode
    {
        public ShapeStyleViewModel Style { get; set; }
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
            Fill = SkiaSharpDrawUtil.ToSKPaintBrush(Style.Fill.Color);
            Stroke = SkiaSharpDrawUtil.ToSKPaintPen(Style, Style.Stroke.Thickness);
        }

        public virtual void Draw(object dc, double zoom)
        {
            var scale = ScaleSize ? 1.0 / zoom : 1.0;
            var translateX = 0.0 - (Center.X * scale) + Center.X;
            var translateY = 0.0 - (Center.Y * scale) + Center.Y;

            double thickness = Style.Stroke.Thickness;

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
