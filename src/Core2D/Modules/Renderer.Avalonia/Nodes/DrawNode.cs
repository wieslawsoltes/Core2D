using System;
using Core2D.Model.Renderer.Nodes;
using Core2D.ViewModels.Style;
using A = Avalonia;
using AM = Avalonia.Media;
using ACP = Avalonia.Controls.PanAndZoom;

namespace Core2D.Renderer
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

            var context = dc as AM.DrawingContext;
            var translateDisposable = scale != 1.0 ? context.PushPreTransform(ACP.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? context.PushPreTransform(ACP.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            OnDraw(dc, zoom);

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
        }

        public abstract void OnDraw(object dc, double zoom);

        public virtual void Dispose()
        {
        }
    }
}
