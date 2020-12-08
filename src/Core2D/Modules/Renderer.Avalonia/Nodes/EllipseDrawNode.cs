using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.Renderer
{
    internal class EllipseDrawNode : DrawNode, IEllipseDrawNode
    {
        public EllipseShape Ellipse { get; set; }
        public A.Rect Rect { get; set; }
        public AM.Geometry Geometry { get; set; }

        public EllipseDrawNode(EllipseShape ellipse, ShapeStyle style)
            : base()
        {
            Style = style;
            Ellipse = ellipse;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Ellipse.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Ellipse.State.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(Ellipse);
            Rect = Geometry.Bounds;
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AM.DrawingContext;

            context.DrawGeometry(Ellipse.IsFilled ? Fill : null, Ellipse.IsStroked ? Stroke : null, Geometry);
        }
    }
}
