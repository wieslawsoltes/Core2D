using Core2D.Shapes;
using Core2D.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class RectangleDrawNode : TextDrawNode, IRectangleDrawNode
    {
        public IRectangleShape Rectangle { get; set; }

        public RectangleDrawNode(IRectangleShape rectangle, IShapeStyle style)
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

            if (Rectangle.IsStroked && Rectangle.IsGrid)
            {
                float ox = Rect.Left;
                float oy = Rect.Top;
                float sx = ox + (float)Rectangle.OffsetX;
                float sy = oy + (float)Rectangle.OffsetY;
                float ex = ox + Rect.Width;
                float ey = oy + Rect.Height;

                for (float x = sx; x < ex; x += (float)Rectangle.CellWidth)
                {
                    var p0 = new SKPoint(x, oy);
                    var p1 = new SKPoint(x, ey);
                    canvas.DrawLine(p0, p1, Stroke);
                }

                for (float y = sy; y < ey; y += (float)Rectangle.CellHeight)
                {
                    var p0 = new SKPoint(ox, y);
                    var p1 = new SKPoint(ex, y);
                    canvas.DrawLine(p0, p1, Stroke);
                }
            }

            base.OnDraw(dc, zoom);
        }
    }
}
