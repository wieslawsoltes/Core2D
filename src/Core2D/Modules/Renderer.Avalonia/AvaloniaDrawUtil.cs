using System;
using Core2D.Style;
using AM = Avalonia.Media;

namespace Core2D.Renderer
{
    internal static class AvaloniaDrawUtil
    {
        public static AM.Color ToColor(ArgbColor argbColor)
        {
            return AM.Color.FromArgb(argbColor.A, argbColor.R, argbColor.G, argbColor.B);
        }

        public static AM.IBrush ToBrush(BaseColor color) => color switch
        {
            ArgbColor argbColor => new AM.Immutable.ImmutableSolidColorBrush(ToColor(argbColor)),
            _ => throw new NotSupportedException($"The {color.GetType()} color type is not supported.")
        };

        public static AM.IPen ToPen(ShapeStyle style, double thickness)
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

        public static AM.IPen ToPen(BaseColor color, double thickness)
        {
            var dashStyle = default(AM.Immutable.ImmutableDashStyle);
            var lineCap = AM.PenLineCap.Flat;
            var brush = ToBrush(color);
            var pen = new AM.Immutable.ImmutablePen(brush, thickness, dashStyle, lineCap);
            return pen;
        }
    }
}
