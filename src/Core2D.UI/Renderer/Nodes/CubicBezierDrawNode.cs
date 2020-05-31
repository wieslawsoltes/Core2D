using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using AM = Avalonia.Media;

namespace Core2D.UI.Renderer
{
    internal class CubicBezierDrawNode : DrawNode
    {
        public ICubicBezierShape CubicBezier { get; set; }
        public AM.Geometry Geometry { get; set; }

        public CubicBezierDrawNode(ICubicBezierShape cubicBezier, IShapeStyle style)
        {
            Style = style;
            CubicBezier = cubicBezier;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = CubicBezier.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = CubicBezier.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(CubicBezier, 0, 0);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            context.DrawGeometry(CubicBezier.IsFilled ? Fill : null, CubicBezier.IsStroked ? Stroke : null, Geometry);
        }
    }
}
