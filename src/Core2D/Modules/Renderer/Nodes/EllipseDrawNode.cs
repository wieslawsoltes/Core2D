#nullable disable
using Core2D.Model.Renderer;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Nodes
{
    internal class EllipseDrawNode : DrawNode, IEllipseDrawNode
    {
        public EllipseShapeViewModel Ellipse { get; set; }
        public A.Rect Rect { get; set; }
#if CUSTOM_DRAW
        public AP.IGeometryImpl Geometry { get; set; }
#else
        public AM.Geometry Geometry { get; set; }
#endif
        public EllipseDrawNode(EllipseShapeViewModel ellipse, ShapeStyleViewModel style)
            : base()
        {
            Style = style;
            Ellipse = ellipse;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Ellipse.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Ellipse.State.HasFlag(ShapeStateFlags.Size);
#if CUSTOM_DRAW
            Geometry = PathGeometryConverter.ToGeometryImpl(Ellipse);
#else
            Geometry = PathGeometryConverter.ToGeometry(Ellipse);
#endif
            Rect = Geometry.Bounds;
            Center = Geometry.Bounds.Center;
        }

        public override void OnDraw(object dc, double zoom)
        {
#if CUSTOM_DRAW
            var context = dc as AP.IDrawingContextImpl;
            context.DrawGeometry(Ellipse.IsFilled ? Fill : null, Ellipse.IsStroked ? Stroke : null, Geometry);
#else
            var context = dc as AM.DrawingContext;
            context.DrawGeometry(Ellipse.IsFilled ? Fill : null, Ellipse.IsStroked ? Stroke : null, Geometry);
#endif
        }
    }
}
