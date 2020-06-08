using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using AM = Avalonia.Media;

namespace Core2D.UI.Renderer
{
    internal class QuadraticBezierDrawNode : DrawNode, IQuadraticBezierDrawNode
    {
        public IQuadraticBezierShape QuadraticBezier { get; set; }
        public AM.Geometry Geometry { get; set; }

        public QuadraticBezierDrawNode(IQuadraticBezierShape quadraticBezier, IShapeStyle style)
        {
            Style = style;
            QuadraticBezier = quadraticBezier;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = QuadraticBezier.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = QuadraticBezier.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(QuadraticBezier);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AM.DrawingContext;

            context.DrawGeometry(QuadraticBezier.IsFilled ? Fill : null, QuadraticBezier.IsStroked ? Stroke : null, Geometry);
        }
    }
}
