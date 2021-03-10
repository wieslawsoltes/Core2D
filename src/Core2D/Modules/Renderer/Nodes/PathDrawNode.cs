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
#if CUSTOM_DRAW
        public AP.IGeometryImpl Geometry { get; set; }
#else
        public AM.Geometry Geometry { get; set; }
#endif
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
#if CUSTOM_DRAW
            Geometry = PathGeometryConverter.ToGeometryImpl(Path.Geometry, Path.IsFilled);
#else
            Geometry = PathGeometryConverter.ToGeometry(Path.Geometry, Path.IsFilled);
#endif
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
#if CUSTOM_DRAW
            var context = dc as AP.IDrawingContextImpl;
            context.DrawGeometry(Path.IsFilled ? Fill : null, Path.IsStroked ? Stroke : null, Geometry);
#else
            var context = dc as AM.DrawingContext;
            context.DrawGeometry(Path.IsFilled ? Fill : null, Path.IsStroked ? Stroke : null, Geometry);
#endif
        }
    }
}
