using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.Renderer
{
    internal abstract class Marker : IMarker
    {
        public BaseShapeViewModel ShapeViewModel { get; set; }
        public ShapeStyleViewModel ShapeStyleViewModel { get; set; }
        public ArrowStyleViewModel Style { get; set; }
        public Avalonia.Media.IBrush Brush { get; set; }
        public Avalonia.Media.IPen Pen { get; set; }
        public Avalonia.Matrix Rotation { get; set; }
        public Avalonia.Point Point { get; set; }

        public abstract void Draw(object dc);

        public virtual void UpdateStyle()
        {
            Brush = AvaloniaDrawUtil.ToBrush(ShapeStyleViewModel.Fill.Color);
            Pen = AvaloniaDrawUtil.ToPen(ShapeStyleViewModel, ShapeStyleViewModel.Stroke.Thickness);
        }
    }
}
