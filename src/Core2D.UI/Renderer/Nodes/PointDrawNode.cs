using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;
using A = Avalonia;
using AM = Avalonia.Media;

namespace Core2D.UI.Renderer
{
    internal class PointDrawNode : DrawNode, IPointDrawNode
    {
        public IPointShape Point { get; set; }
        public double PointSize { get; set; }
        public A.Rect Rect { get; set; }

        public PointDrawNode(IPointShape point, IShapeStyle pointStyle, double pointSize)
        {
            Style = pointStyle;
            Point = point;
            PointSize = pointSize;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = true; // Point.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = true; // Point.State.Flags.HasFlag(ShapeStateFlags.Size);
            var rect2 = Rect2.FromPoints(Point.X - PointSize, Point.Y - PointSize, Point.X + PointSize, Point.Y + PointSize, 0, 0);
            Rect = new A.Rect(rect2.X, rect2.Y, rect2.Width, rect2.Height);
            Center = Rect.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AM.DrawingContext;

            context.FillRectangle(Fill, Rect);
            context.DrawRectangle(Stroke, Rect);
        }
    }
}
