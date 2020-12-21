#nullable disable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using SkiaSharp;

namespace Core2D.Modules.Renderer.SkiaSharp.Nodes
{
    internal class PathDrawNode : DrawNode, IPathDrawNode
    {
        public PathShapeViewModel Path { get; set; }
        public SKPath Geometry { get; set; }

        public PathDrawNode(PathShapeViewModel path, ShapeStyleViewModel style)
        {
            Style = style;
            Path = path;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Path.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Path.State.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToSKPath(Path.Geometry);
            Center = new SKPoint(Geometry.Bounds.MidX, Geometry.Bounds.MidY);
        }

        public override void OnDraw(object dc, double zoom)
        {
            var canvas = dc as SKCanvas;

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
}
