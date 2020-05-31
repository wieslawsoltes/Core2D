using System;
using Core2D.Style;
using A = Avalonia;
using AM = Avalonia.Media;
using AME = Avalonia.MatrixExtensions;

namespace Core2D.UI.Renderer
{
    internal abstract class DrawNode : IDisposable
    {
        public IShapeStyle Style { get; set; }
        public bool ScaleThickness { get; set; }
        public bool ScaleSize { get; set; }
        public AM.IBrush Fill { get; set; }
        public AM.IPen Stroke { get; set; }
        public A.Point Center { get; set; }

        protected AM.Color ToColor(IArgbColor argbColor)
        {
            return AM.Color.FromArgb(argbColor.A, argbColor.R, argbColor.G, argbColor.B);
        }

        protected AM.IBrush ToBrush(IColor color) => color switch
        {
            IArgbColor argbColor => new AM.Immutable.ImmutableSolidColorBrush(ToColor(argbColor)),
            _ => throw new NotSupportedException($"The {color.GetType()} color type is not supported.")
        };

        protected AM.IPen ToPen(IBaseStyle style, double thickness)
        {
            var dashStyle = default(AM.Immutable.ImmutableDashStyle);
            if (style.Dashes != null)
            {
                var dashes = StyleHelper.ConvertDashesToDoubleArray(style.Dashes, 1.0);
                var dashOffset = style.DashOffset;
                if (dashes != null)
                {
                    dashStyle = new AM.Immutable.ImmutableDashStyle(dashes, dashOffset);
                }
            }

            var lineCap = style.LineCap switch
            {
                LineCap.Flat => AM.PenLineCap.Flat,
                LineCap.Square => AM.PenLineCap.Square,
                LineCap.Round => AM.PenLineCap.Round,
                _ => throw new NotImplementedException()
            };

            var brush = ToBrush(style.Stroke);
            var pen = new AM.Immutable.ImmutablePen(brush, thickness, dashStyle, lineCap);

            return pen;
        }

        public DrawNode()
        {
        }

        public abstract void UpdateGeometry();

        public virtual void UpdateStyle()
        {
            Fill = ToBrush(Style.Fill);
            Stroke = ToPen(Style, Style.Thickness);
        }

        public virtual void Draw(AM.DrawingContext context, double dx, double dy, double zoom)
        {
            var scale = ScaleSize ? 1.0 / zoom : 1.0;
            var translateX = 0.0 - (Center.X * scale) + Center.X;
            var translateY = 0.0 - (Center.Y * scale) + Center.Y;

            double thickness = Style.Thickness;

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
                Stroke = ToPen(Style, thickness);
            }

            var offsetDisposable = dx != 0.0 || dy != 0.0 ? context.PushPreTransform(AME.MatrixHelper.Translate(dx, dy)) : default(IDisposable);
            var translateDisposable = scale != 1.0 ? context.PushPreTransform(AME.MatrixHelper.Translate(translateX, translateY)) : default(IDisposable);
            var scaleDisposable = scale != 1.0 ? context.PushPreTransform(AME.MatrixHelper.Scale(scale, scale)) : default(IDisposable);

            OnDraw(context, dx, dy, zoom);

            scaleDisposable?.Dispose();
            translateDisposable?.Dispose();
            offsetDisposable?.Dispose();
        }

        public abstract void OnDraw(AM.DrawingContext context, double dx, double dy, double zoom);

        public virtual void Dispose()
        {
        }
    }
}
