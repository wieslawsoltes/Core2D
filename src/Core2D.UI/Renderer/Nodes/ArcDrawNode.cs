using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using AM = Avalonia.Media;

namespace Core2D.UI.Renderer
{
    internal class ArcDrawNode : DrawNode
    {
        public IArcShape Arc { get; set; }
        public AM.Geometry Geometry { get; set; }

        public ArcDrawNode(IArcShape arc, IShapeStyle style)
        {
            Style = style;
            Arc = arc;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Arc.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Arc.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(Arc, 0, 0);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            context.DrawGeometry(Arc.IsFilled ? Fill : null, Arc.IsStroked ? Stroke : null, Geometry);
        }
    }
}
