#nullable disable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Nodes
{
    internal class PathDrawNode : DrawNode, IPathDrawNode
    {
        public PathShapeViewModel Path { get; set; }
        public AM.Geometry Geometry { get; set; }

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
            Geometry = PathGeometryConverter.ToStreamGeometry(Path.Geometry);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
#if CUSTOM_DRAW
            var context = dc as AP.IDrawingContextImpl;
            context.DrawGeometry(Path.IsFilled ? Fill : null, Path.IsStroked ? Stroke : null, Geometry.PlatformImpl);
#else
            var context = dc as AM.DrawingContext;
            context.DrawGeometry(Path.IsFilled ? Fill : null, Path.IsStroked ? Stroke : null, Geometry);
#endif
        }
    }
}
