#nullable enable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes;

internal class PathDrawNode : DrawNode, IPathDrawNode
{
    public PathShapeViewModel Path { get; set; }
    public SKPath? Geometry { get; set; }

    public PathDrawNode(PathShapeViewModel path, ShapeStyleViewModel? style)
    {
        Style = style;
        Path = path;
        UpdateGeometry();
    }

    public sealed override void UpdateGeometry()
    {
        ScaleThickness = Path.State.HasFlag(ShapeStateFlags.Thickness);
        ScaleSize = Path.State.HasFlag(ShapeStateFlags.Size);
        Geometry = PathGeometryConverter.ToSKPath(Path);
        Center = new SKPoint(Geometry.Bounds.MidX, Geometry.Bounds.MidY);
    }

    public override void OnDraw(object? dc, double zoom)
    {
        if (dc is not SKCanvas canvas)
        {
            return;
        }

        if (Path.IsFilled)
        {
            canvas.DrawPath(Geometry, Fill);
        }

        if (Path.IsStroked)
        {
            canvas.DrawPath(Geometry, Stroke);
        }
    }
}
