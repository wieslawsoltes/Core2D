#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal class CubicBezierDrawNode : DrawNode, ICubicBezierDrawNode
{
    public CubicBezierShapeViewModel CubicBezier { get; set; }
    public AM.Geometry? Geometry { get; set; }

    public CubicBezierDrawNode(CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel? style)
    {
        Style = style;
        CubicBezier = cubicBezier;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = CubicBezier.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = CubicBezier.State.HasFlag(ShapeStateFlags.Size);
        Geometry = PathGeometryConverter.ToGeometryImpl(CubicBezier);
        Center = Geometry is { } ? Geometry.Bounds.Center : new A.Point();
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not AM.ImmediateDrawingContext context)
        {
            return;
        }

        if (Geometry is { })
        {
            // TODO: context.DrawGeometry(CubicBezier.IsFilled ? Fill : null, CubicBezier.IsStroked ? Stroke : null, Geometry);
        }
    }
}
