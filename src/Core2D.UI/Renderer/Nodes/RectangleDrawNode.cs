using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.UI.Renderer
{
    internal class RectangleDrawNode : TextDrawNode
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
            ScaleThickness = Rectangle.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Rectangle.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect2 = Rect2.FromPoints(Rectangle.TopLeft.X, Rectangle.TopLeft.Y, Rectangle.BottomRight.X, Rectangle.BottomRight.Y, 0, 0);
            Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = Rect.Center;

            base.UpdateTextGeometry();
        }

        public override void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            if (Rectangle.IsFilled)
            {
                context.FillRectangle(Fill, Rect);
            }

            if (Rectangle.IsStroked)
            {
                context.DrawRectangle(Stroke, Rect);
            }

            if (Rectangle.IsStroked && Rectangle.IsGrid)
            {
                double ox = Rect.X;
                double oy = Rect.Y;
                double sx = ox + Rectangle.OffsetX;
                double sy = oy + Rectangle.OffsetY;
                double ex = ox + Rect.Width;
                double ey = oy + Rect.Height;

                for (double x = sx; x < ex; x += Rectangle.CellWidth)
                {
                    var p0 = new A.Point(x, oy);
                    var p1 = new A.Point(x, ey);
                    context.DrawLine(Stroke, p0, p1);

                }

                for (double y = sy; y < ey; y += Rectangle.CellHeight)
                {
                    var p0 = new A.Point(ox, y);
                    var p1 = new A.Point(ex, y);
                    context.DrawLine(Stroke, p0, p1);
                }
            }

            base.OnDraw(context, dx, dy, zoom);
        }
    }
}
