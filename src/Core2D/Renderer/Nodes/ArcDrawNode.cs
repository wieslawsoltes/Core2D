using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using AM = Avalonia.Media;

namespace Core2D.UI.Renderer
{
    internal class ArcDrawNode : DrawNode, IArcDrawNode
    {
        public ArcShape Arc { get; set; }
        public AM.Geometry Geometry { get; set; }

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
            Geometry = PathGeometryConverter.ToGeometry(Arc);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AM.DrawingContext;

            context.DrawGeometry(Arc.IsFilled ? Fill : null, Arc.IsStroked ? Stroke : null, Geometry);
        }
    }
}
