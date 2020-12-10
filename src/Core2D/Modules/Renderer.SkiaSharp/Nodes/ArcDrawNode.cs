using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class ArcDrawNode : DrawNode, IArcDrawNode
    {
        public ArcShapeViewModelViewModel Arc { get; set; }
        public SKPath Geometry { get; set; }

        public ArcDrawNode(ArcShapeViewModelViewModel arc, ShapeStyleViewModel style)
        {
            Style = style;
            Arc = arc;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Arc.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Arc.State.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToSKPath(Arc);
            Center = new SKPoint(Geometry.Bounds.MidX, Geometry.Bounds.MidY);
        }

        public override void OnDraw(object dc, double zoom)
        {
            var canvas = dc as SKCanvas;

            if (Arc.IsFilled)
            {
                canvas.DrawPath(Geometry, Fill);
            }

            if (Arc.IsStroked)
            {
                canvas.DrawPath(Geometry, Stroke);
            }
        }
    }
}
