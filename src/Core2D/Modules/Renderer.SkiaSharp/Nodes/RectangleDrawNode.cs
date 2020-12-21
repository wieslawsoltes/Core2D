#nullable disable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;
using Spatial;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes
{
    internal class RectangleDrawNode : DrawNode, IRectangleDrawNode
    {
        public RectangleShapeViewModel Rectangle { get; set; }
        public SKRect Rect { get; set; }

        public RectangleDrawNode(RectangleShapeViewModel rectangle, ShapeStyleViewModel style)
            : base()
        {
            Style = style;
            Rectangle = rectangle;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Rectangle.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Rectangle.State.HasFlag(ShapeStateFlags.Size);
            var rect2 = Rect2.FromPoints(Rectangle.TopLeft.X, Rectangle.TopLeft.Y, Rectangle.BottomRight.X, Rectangle.BottomRight.Y, 0, 0);
            Rect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);
            Center = new SKPoint(Rect.MidX, Rect.MidY);
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
        }
    }
}
