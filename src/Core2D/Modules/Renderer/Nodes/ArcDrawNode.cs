using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using AM = Avalonia.Media;

namespace Core2D.Modules.Renderer.Nodes
{
    internal class ArcDrawNode : DrawNode, IArcDrawNode
    {
        public ArcShapeViewModelViewModel Arc { get; set; }
        public AM.Geometry Geometry { get; set; }

        public ArcDrawNode(ArcShapeViewModelViewModel arc, ShapeStyleViewModel style)
        {
            Style = style;
            Arc = arc;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Arc.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Arc.State.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToGeometry(Arc);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AM.DrawingContext;

            context.DrawGeometry(Arc.IsFilled ? Fill : null, Arc.IsStroked ? Stroke : null, Geometry);
        }
    }
}
