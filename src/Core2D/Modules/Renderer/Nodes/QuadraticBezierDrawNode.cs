#nullable disable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Nodes
{
    internal class QuadraticBezierDrawNode : DrawNode, IQuadraticBezierDrawNode
    {
        public QuadraticBezierShapeViewModel QuadraticBezier { get; set; }
#if CUSTOM_DRAW
        public AP.IGeometryImpl Geometry { get; set; }
#else
        public AM.Geometry Geometry { get; set; }
#endif
        public QuadraticBezierDrawNode(QuadraticBezierShapeViewModel quadraticBezier, ShapeStyleViewModel style)
        {
            Style = style;
            QuadraticBezier = quadraticBezier;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = QuadraticBezier.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = QuadraticBezier.State.HasFlag(ShapeStateFlags.Size);
#if CUSTOM_DRAW
            Geometry = PathGeometryConverter.ToGeometryImpl(QuadraticBezier);
#else
            Geometry = PathGeometryConverter.ToGeometry(QuadraticBezier);
#endif
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
#if CUSTOM_DRAW
            var context = dc as AP.IDrawingContextImpl;
            context.DrawGeometry(QuadraticBezier.IsFilled ? Fill : null, QuadraticBezier.IsStroked ? Stroke : null, Geometry);
#else
            var context = dc as AM.DrawingContext;
            context.DrawGeometry(QuadraticBezier.IsFilled ? Fill : null, QuadraticBezier.IsStroked ? Stroke : null, Geometry);
#endif
        }
    }
}
