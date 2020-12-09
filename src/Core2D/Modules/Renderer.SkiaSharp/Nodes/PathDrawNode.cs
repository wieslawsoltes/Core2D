using Core2D.Shapes;
using Core2D.Style;
using SkiaSharp;

namespace Core2D.Renderer.SkiaSharp
{
    internal class PathDrawNode : DrawNode, IPathDrawNode
    {
        public PathShapeViewModel Path { get; set; }
        public SKPath Geometry { get; set; }

        public PathDrawNode(PathShapeViewModel path, ShapeStyleViewModel styleViewModel)
        {
            StyleViewModel = styleViewModel;
            Path = path;
            UpdateGeometry();
        }

        public override void UpdateGeometry()
        {
            ScaleThickness = Path.State.HasFlag(ShapeStateFlags.Thickness);
            ScaleSize = Path.State.HasFlag(ShapeStateFlags.Size);
            Geometry = PathGeometryConverter.ToSKPath(Path.GeometryViewModel);
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
