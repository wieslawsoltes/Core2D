using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using AM = Avalonia.Media;

namespace Core2D.UI.Renderer
{
    internal class EllipseDrawNode : TextDrawNode, IEllipseDrawNode
    {
        public EllipseShape Ellipse { get; set; }
        public AM.Geometry Geometry { get; set; }

        public EllipseDrawNode(EllipseShape ellipse, ShapeStyle style)
            : base()
        {
            Style = style;
            Ellipse = ellipse;
            Text = ellipse;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Ellipse.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Ellipse.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(Ellipse);
            Rect = Geometry.Bounds;
            Center = Geometry.Bounds.Center;

            base.UpdateTextGeometry();
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AM.DrawingContext;

            context.DrawGeometry(Ellipse.IsFilled ? Fill : null, Ellipse.IsStroked ? Stroke : null, Geometry);

            base.OnDraw(dc, zoom);
        }
    }
}
