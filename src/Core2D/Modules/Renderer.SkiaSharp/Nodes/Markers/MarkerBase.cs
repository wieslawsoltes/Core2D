#nullable disable
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes.Marker
{
    internal abstract class MarkerBase : IMarker
    {
        public BaseShapeViewModel ShapeViewModel { get; set; }
        public ShapeStyleViewModel ShapeStyleViewModel { get; set; }
        public ArrowStyleViewModel Style { get; set; }
        public SKPaint Brush { get; set; }
        public SKPaint Pen { get; set; }
        public SKMatrix Rotation { get; set; }
        public SKPoint Point { get; set; }

        public abstract void Draw(object dc);

        public virtual void UpdateStyle()
        {
            Brush = SkiaSharpDrawUtil.ToSKPaintBrush(ShapeStyleViewModel.Fill.Color);
            Pen = SkiaSharpDrawUtil.ToSKPaintPen(ShapeStyleViewModel, ShapeStyleViewModel.Stroke.Thickness);
        }
    }
}
