using Core2D.Shapes;
using Core2D.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class RectangleDrawNode : TextDrawNode, IRectangleDrawNode
    {
        public RectangleShape Rectangle { get; set; }

        public RectangleDrawNode(RectangleShape rectangle, ShapeStyle style)
            : base()
        {
            Style = style;
            Rectangle = rectangle;
            Text = rectangle;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            base.UpdateGeometry();
        }

        public override void OnDraw(object dc, double zoom)
        {
            var canvas = dc as SKCanvas;

            if (Rectangle.IsFilled)
            {
                canvas.DrawRect(Rect, Fill);
            }

            if (Rectangle.IsStroked)
            {
                canvas.DrawRect(Rect, Stroke);
            }

            base.OnDraw(dc, zoom);
        }
    }
}
