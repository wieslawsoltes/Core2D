using Core2D.Shapes;
using Core2D.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class ArcDrawNode : DrawNode, IArcDrawNode
    {
        public ArcShape Arc { get; set; }
        public SKPath Geometry { get; set; }

        public ArcDrawNode(ArcShape arc, ShapeStyle style)
        {
            Style = style;
            Arc = arc;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Arc.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Arc.State.Flags.HasFlag(ShapeStateFlags.Size);
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
