using System;
using Core2D.Style;
using AM = Avalonia.Media;

namespace Core2D.UI.Renderer
{
    internal static class AvaloniaDrawUtil
    {
        public static AM.Color ToColor(IArgbColor argbColor)
        {
            return AM.Color.FromArgb(argbColor.A, argbColor.R, argbColor.G, argbColor.B);
        }

        public static AM.IBrush ToBrush(IColor color) => color switch
        {
            IArgbColor argbColor => new AM.Immutable.ImmutableSolidColorBrush(ToColor(argbColor)),
            _ => throw new NotSupportedException($"The {color.GetType()} color type is not supported.")
        };

        public static AM.IPen ToPen(IBaseStyle style, double thickness)
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
    }
}
