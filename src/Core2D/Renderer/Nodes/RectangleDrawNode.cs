using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.Renderer
{
    internal class RectangleDrawNode : DrawNode, IRectangleDrawNode
    {
        public RectangleShape Rectangle { get; set; }
        public A.Rect Rect { get; set; }

        public RectangleDrawNode(RectangleShape rectangle, ShapeStyle style)
            : base()
        {
            Style = style;
            Rectangle = rectangle;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Rectangle.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Rectangle.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect2 = Rect2.FromPoints(Rectangle.TopLeft.X, Rectangle.TopLeft.Y, Rectangle.BottomRight.X, Rectangle.BottomRight.Y, 0, 0);
            Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = Rect.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AM.DrawingContext;

            if (Rectangle.IsFilled)
            {
                context.FillRectangle(Fill, Rect);
            }

            if (Rectangle.IsStroked)
            {
                context.DrawRectangle(Stroke, Rect);
            }
        }
    }
}
