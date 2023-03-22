#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes;

internal class EllipseDrawNode : DrawNode, IEllipseDrawNode
{
    public EllipseShapeViewModel Ellipse { get; set; }
    public A.Rect Rect { get; set; }
    public AP.IGeometryImpl? Geometry { get; set; }

    public EllipseDrawNode(EllipseShapeViewModel ellipse, ShapeStyleViewModel? style)
    {
        Style = style;
        Ellipse = ellipse;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = Ellipse.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Ellipse.State.HasFlag(ShapeStateFlags.Size);
        Geometry = PathGeometryConverter.ToGeometryImpl(Ellipse);

        if (Geometry is { })
        {
            Rect = Geometry.Bounds;
            Center = Geometry.Bounds.Center;
        }
        else
        {
            Rect = new A.Rect();
            Center = new A.Point();
        }
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not AP.IDrawingContextImpl context)
        {
            return;
        }

        if (Geometry is { })
        {
            context.DrawGeometry(Ellipse.IsFilled ? Fill : null, Ellipse.IsStroked ? Stroke : null, Geometry);
        }
    }
}
