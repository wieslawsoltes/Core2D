using Core2D.Shapes;
using Core2D.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal abstract class Marker : IMarker
    {
        public BaseShapeViewModel ShapeViewModel { get; set; }
        public ShapeStyleViewModel ShapeStyleViewModel { get; set; }
        public ArrowStyleViewModel StyleViewModel { get; set; }
        public SKPaint Brush { get; set; }
        public SKPaint Pen { get; set; }
        public SKMatrix Rotation { get; set; }
        public SKPoint Point { get; set; }

        public abstract void Draw(object dc);

        public virtual void UpdateStyle()
        {
            Brush = SkiaSharpDrawUtil.ToSKPaintBrush(ShapeStyleViewModel.Fill.ColorViewModel);
            Pen = SkiaSharpDrawUtil.ToSKPaintPen(ShapeStyleViewModel, ShapeStyleViewModel.Stroke.Thickness);
        }
    }
}
