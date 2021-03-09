#nullable disable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Nodes
{
    internal class ArcDrawNode : DrawNode, IArcDrawNode
    {
        public ArcShapeViewModel Arc { get; set; }
#if CUSTOM_DRAW
        public AP.IGeometryImpl Geometry { get; set; }
#else
        public AM.Geometry Geometry { get; set; }
#endif
        public ArcDrawNode(ArcShapeViewModel arc, ShapeStyleViewModel style)
        {
            Style = style;
            Arc = arc;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Arc.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Arc.State.HasFlag(ShapeStateFlags.Size);
#if CUSTOM_DRAW
            Geometry = PathGeometryConverter.ToGeometryImpl(Arc);
#else
            Geometry = PathGeometryConverter.ToGeometry(Arc);
#endif
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
#if CUSTOM_DRAW
            var context = dc as AP.IDrawingContextImpl;
            context.DrawGeometry(Arc.IsFilled ? Fill : null, Arc.IsStroked ? Stroke : null, Geometry);
#else
            var context = dc as AM.DrawingContext;
            context.DrawGeometry(Arc.IsFilled ? Fill : null, Arc.IsStroked ? Stroke : null, Geometry);
#endif
        }
    }
}
