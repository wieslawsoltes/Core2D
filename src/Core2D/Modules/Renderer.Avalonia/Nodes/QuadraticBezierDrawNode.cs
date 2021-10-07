#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal class QuadraticBezierDrawNode : DrawNode, IQuadraticBezierDrawNode
{
    public QuadraticBezierShapeViewModel QuadraticBezier { get; set; }
    public AP.IGeometryImpl Geometry { get; set; }

    public QuadraticBezierDrawNode(QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel style)
    {
        Style = style;
        QuadraticBezier = quadraticBezier;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = QuadraticBezier.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = QuadraticBezier.State.HasFlag(ShapeStateFlags.Size);
        Geometry = PathGeometryConverter.ToGeometryImpl(QuadraticBezier);
        Center = Geometry.Bounds.Center;
    }

    public override void OnDraw(object dc, double zoom)
    {
        var context = dc as AP.IDrawingContextImpl;
        context.DrawGeometry(QuadraticBezier.IsFilled ? Fill : null, QuadraticBezier.IsStroked ? Stroke : null, Geometry);
    }
}