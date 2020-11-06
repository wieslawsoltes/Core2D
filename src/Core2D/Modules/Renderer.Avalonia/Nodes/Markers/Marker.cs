using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    internal abstract class Marker : IMarker
    {
        public BaseShape Shape { get; set; }
        public BaseStyle BaseStyle { get; set; }
        public ArrowStyle Style { get; set; }
        public Avalonia.Media.IBrush Brush { get; set; }
        public Avalonia.Media.IPen Pen { get; set; }
        public Avalonia.Matrix Rotation { get; set; }
        public Avalonia.Point Point { get; set; }

        public abstract void Draw(object dc);

        public virtual void UpdateStyle()
        {
            Brush = AvaloniaDrawUtil.ToBrush(BaseStyle.Fill);
            Pen = AvaloniaDrawUtil.ToPen(BaseStyle, BaseStyle.Thickness);
        }
    }
}
