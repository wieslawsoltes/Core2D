using Core2D.Shapes;
using Core2D.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class CubicBezierDrawNode : DrawNode, ICubicBezierDrawNode
    {
        public CubicBezierShape CubicBezier { get; set; }
        public SKPath Geometry { get; set; }

        public CubicBezierDrawNode(CubicBezierShape cubicBezier, ShapeStyle style)
        {
            Style = style;
            CubicBezier = cubicBezier;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = CubicBezier.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = CubicBezier.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToSKPath(CubicBezier);
            Center = new SKPoint(Geometry.Bounds.MidX, Geometry.Bounds.MidY);
        }

        public override void OnDraw(object dc, double zoom)
        {
            var canvas = dc as SKCanvas;

            if (CubicBezier.IsFilled)
            {
                canvas.DrawPath(Geometry, Fill);
            }

            if (CubicBezier.IsStroked)
            {
                canvas.DrawPath(Geometry, Stroke);
            }
        }
    }
}
