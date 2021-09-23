#nullable enable
using Core2D.Model.Renderer.Nodes;
using Core2D.Modules.Renderer.Avalonia.Media;
using Core2D.ViewModels.Style;
using A = Avalonia;
using ACP = Avalonia.Controls.PanAndZoom;
using AM = Avalonia.Media;
using AP = Avalonia.Platform;

namespace Core2D.Modules.Renderer.Avalonia.Nodes
{
    internal abstract class DrawNode : IDrawNode
    {
        public ShapeStyleViewModel Style { get; set; }
        public bool ScaleThickness { get; set; }
        public bool ScaleSize { get; set; }
        public AM.IBrush Fill { get; set; }
        public AM.IPen Stroke { get; set; }
        public A.Point Center { get; set; }

        public DrawNode()
        {
        }

        public abstract void UpdateGeometry();

        public virtual void UpdateStyle()
        {
            Fill = AvaloniaDrawUtil.ToBrush(Style.Fill.Color);
            Stroke = AvaloniaDrawUtil.ToPen(Style, Style.Stroke.Thickness);
        }

        public virtual void Draw(object dc, double zoom)
        {
            var scale = ScaleSize ? 1.0 / zoom : 1.0;
            var translateX = 0.0 - (Center.X * scale) + Center.X;
            var translateY = 0.0 - (Center.Y * scale) + Center.Y;

            double thickness = Style.Stroke.Thickness;

            if (ScaleThickness)
            {
                thickness /= zoom;
            }

            if (scale != 1.0)
            {
                thickness /= scale;
            }

            if (Stroke.Thickness != thickness)
            {
                Stroke = AvaloniaDrawUtil.ToPen(Style, thickness);
            }

            var context = dc as AP.IDrawingContextImpl;
            if (scale != 1.0)
            {
                using var translateDisposable = context.PushPreTransform(ACP.MatrixHelper.Translate(translateX, translateY));
                using var scaleDisposable =  context.PushPreTransform(ACP.MatrixHelper.Scale(scale, scale));
                OnDraw(dc, zoom);
            }
            else
            {
                OnDraw(dc, zoom);
            }
        }

        public abstract void OnDraw(object dc, double zoom);

        public virtual void Dispose()
        {
        }
    }
}
