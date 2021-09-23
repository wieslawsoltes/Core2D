#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.Spatial;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes
{
    internal class RectangleDrawNode : DrawNode, IRectangleDrawNode
    {
        public RectangleShapeViewModel Rectangle { get; set; }
        public A.Rect Rect { get; set; }

        public RectangleDrawNode(RectangleShapeViewModel rectangle, ShapeStyleViewModel style)
            : base()
        {
            Style = style;
            Rectangle = rectangle;
            UpdateGeometry();
        }

        public sealed override void UpdateGeometry()
        {
            ScaleThickness = Rectangle.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Rectangle.State.HasFlag(ShapeStateFlags.Size);
            var rect2 = Rect2.FromPoints(Rectangle.TopLeft.X, Rectangle.TopLeft.Y, Rectangle.BottomRight.X, Rectangle.BottomRight.Y, 0, 0);
            Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = Rect.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AP.IDrawingContextImpl;
            if (Rectangle.IsFilled)
            {
                context.DrawRectangle(Fill, null, Rect);
            }

            if (Rectangle.IsStroked)
            {
                context.DrawRectangle(null, Stroke, Rect);
            }
        }
    }
}
