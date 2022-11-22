#nullable enable
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;
using Core2D.Spatial;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class PointDrawNode : DrawNode, IPointDrawNode
{
    public PointShapeViewModel Point { get; set; }
    public double PointSize { get; set; }
    public SKRect Rect { get; set; }

    public PointDrawNode(PointShapeViewModel point, ShapeStyleViewModel? pointStyleViewModel, double pointSize)
    {
        Style = pointStyleViewModel;
        Point = point;
        PointSize = pointSize;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = true; // Point.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = true; // Point.State.HasFlag(ShapeStateFlags.Size);
        var rect2 = Rect2.FromPoints(Point.X - PointSize, Point.Y - PointSize, Point.X + PointSize, Point.Y + PointSize);
        Rect = SKRect.Create((float)rect2.X, (float)rect2.Y, (float)rect2.Width, (float)rect2.Height);
        Center = new SKPoint(Rect.MidX, Rect.MidY);
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        canvas.DrawRect(Rect, Fill);
        canvas.DrawRect(Rect, Stroke);
    }
}
