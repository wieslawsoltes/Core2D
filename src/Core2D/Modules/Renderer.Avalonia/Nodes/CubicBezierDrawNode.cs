#nullable disable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes
{
    internal class CubicBezierDrawNode : DrawNode, ICubicBezierDrawNode
    {
        public CubicBezierShapeViewModel CubicBezier { get; set; }
        public AP.IGeometryImpl Geometry { get; set; }

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
            Geometry = PathGeometryConverter.ToGeometryImpl(CubicBezier);
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
            var context = dc as AP.IDrawingContextImpl;
            context.DrawGeometry(CubicBezier.IsFilled ? Fill : null, CubicBezier.IsStroked ? Stroke : null, Geometry);
        }
    }
}
