using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using AM = Avalonia.Media;

namespace Core2D.UI.Renderer
{
    internal class PathDrawNode : DrawNode
    {
        public IPathShape Path { get; set; }
        public AM.Geometry Geometry { get; set; }

        public PathDrawNode(IPathShape path, IShapeStyle style)
        {
            Style = style;
            Path = path;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Path.State.Flags.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Path.State.Flags.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(Path.Geometry);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(AM.DrawingContext context, double zoom)
        {
            context.DrawGeometry(Path.IsFilled ? Fill : null, Path.IsStroked ? Stroke : null, Geometry);
        }
    }
}
