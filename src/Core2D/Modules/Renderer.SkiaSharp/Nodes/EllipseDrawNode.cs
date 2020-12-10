using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;
using Spatial;

namespace Core2D.Renderer.SkiaSharp
{
    internal class EllipseDrawNode : DrawNode, IEllipseDrawNode
    {
        public EllipseShapeViewModel Ellipse { get; set; }
        public SKRect Rect { get; set; }

        public EllipseDrawNode(EllipseShapeViewModel ellipse, ShapeStyleViewModel style)
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
            var rect2 = Rect2.FromPoints(Ellipse.TopLeft.X, Ellipse.TopLeft.Y, Ellipse.BottomRight.X, Ellipse.BottomRight.Y, 0, 0);
            Rect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);
            Center = new SKPoint(Rect.MidX, Rect.MidY);
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
        }
    }
}
