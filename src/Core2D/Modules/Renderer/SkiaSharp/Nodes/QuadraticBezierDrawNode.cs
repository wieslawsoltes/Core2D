#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class QuadraticBezierDrawNode : DrawNode, IQuadraticBezierDrawNode
{
    public QuadraticBezierShapeViewModel QuadraticBezier { get; set; }
    public SKPath? Geometry { get; set; }

    public QuadraticBezierDrawNode(QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel? style)
    {
        Style = style;
        QuadraticBezier = quadraticBezier;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = QuadraticBezier.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = QuadraticBezier.State.HasFlag(ShapeStateFlags.Size);
        Geometry = PathGeometryConverter.ToSKPath(QuadraticBezier);
        Center = new SKPoint(Geometry.Bounds.MidX, Geometry.Bounds.MidY);
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        if (QuadraticBezier.IsFilled)
        {
            canvas.DrawPath(Geometry, Fill);
        }

        if (QuadraticBezier.IsStroked)
        {
            canvas.DrawPath(Geometry, Stroke);
        }
    }
}
