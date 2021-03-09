#nullable disable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Nodes
{
    internal class CubicBezierDrawNode : DrawNode, ICubicBezierDrawNode
    {
        public CubicBezierShapeViewModel CubicBezier { get; set; }
#if CUSTOM_DRAW
        public AP.IGeometryImpl Geometry { get; set; }
#else
        public AM.Geometry Geometry { get; set; }
#endif
        public CubicBezierDrawNode(CubicBezierShapeViewModel cubicBezier, ShapeStyleViewModel style)
        {
            Style = style;
            CubicBezier = cubicBezier;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = CubicBezier.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = CubicBezier.State.HasFlag(ShapeStateFlags.Size);
#if CUSTOM_DRAW
            Geometry = PathGeometryConverter.ToGeometryImpl(CubicBezier);
#else
            Geometry = PathGeometryConverter.ToGeometry(CubicBezier);
#endif
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
#if CUSTOM_DRAW
            var context = dc as AP.IDrawingContextImpl;
            context.DrawGeometry(CubicBezier.IsFilled ? Fill : null, CubicBezier.IsStroked ? Stroke : null, Geometry);
#else
            var context = dc as AM.DrawingContext;
            context.DrawGeometry(CubicBezier.IsFilled ? Fill : null, CubicBezier.IsStroked ? Stroke : null, Geometry);
#endif
        }
    }
}
