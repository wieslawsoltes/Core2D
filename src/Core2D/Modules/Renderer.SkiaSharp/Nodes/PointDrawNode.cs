using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class PointDrawNode : DrawNode, IPointDrawNode
    {
        public PointShapeViewModel Point { get; set; }
        public double PointSize { get; set; }
        public SKRect Rect { get; set; }

        public PointDrawNode(PointShapeViewModel point, ShapeStyleViewModel pointStyleViewModel, double pointSize)
        {
            StyleViewModel = pointStyleViewModel;
            Point = point;
            PointSize = pointSize;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = true; // Point.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = true; // Point.State.HasFlag(ShapeStateFlags.Size);
            var rect2 = Rect2.FromPoints(Point.X - PointSize, Point.Y - PointSize, Point.X + PointSize, Point.Y + PointSize, 0, 0);
            Rect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);
            Center = new SKPoint(Rect.MidX, Rect.MidY);
        }

        public override void OnDraw(object dc, double zoom)
        {
            var canvas = dc as SKCanvas;

            canvas.DrawRect(Rect, Fill);
            canvas.DrawRect(Rect, Stroke);
        }
    }
}
