using Core2D.Shapes;
using Core2D.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class EllipseDrawNode : TextDrawNode, IEllipseDrawNode
    {
        public IEllipseShape Ellipse { get; set; }

        public EllipseDrawNode(IEllipseShape ellipse, IShapeStyle style)
            : base()
        {
            Style = style;
            Ellipse = ellipse;
            Text = ellipse;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
        }

        public override void OnDraw(object dc, double zoom)
        {
            var canvas = dc as SKCanvas;

            if (Ellipse.IsFilled)
            {
                canvas.DrawOval(Rect, Fill);
            }

            if (Ellipse.IsStroked)
            {
                canvas.DrawOval(Rect, Stroke);
            }

            base.OnDraw(dc, zoom);
        }
    }
}
