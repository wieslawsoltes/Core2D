using System;
using Core2D.Model.Style;
using Core2D.ViewModels.Style;
using AM = Avalonia.Media;

namespace Core2D.Modules.Renderer
{
    internal static class AvaloniaDrawUtil
    {
        public static AM.Color ToColor(ArgbColorViewModel argbColorViewModelViewModel)
        {
            return AM.Color.FromArgb(argbColorViewModelViewModel.A, argbColorViewModelViewModel.R, argbColorViewModelViewModel.G, argbColorViewModelViewModel.B);
        }

        public static AM.IBrush ToBrush(BaseColorViewModel colorViewModel) => colorViewModel switch
        {
            ArgbColorViewModel argbColor => new AM.Immutable.ImmutableSolidColorBrush(ToColor(argbColor)),
            _ => throw new NotSupportedException($"The {colorViewModel.GetType()} color type is not supported.")
        };

        public static AM.IPen ToPen(ShapeStyleViewModel style, double thickness)
        {
            var dashStyle = default(AM.Immutable.ImmutableDashStyle);
            if (style.Stroke.Dashes is { })
            {
                var dashes = StyleHelper.ConvertDashesToDoubleArray(style.Stroke.Dashes, 1.0);
                var dashOffset = style.Stroke.DashOffset;
                if (dashes is { })
                {
                    dashStyle = new AM.Immutable.ImmutableDashStyle(dashes, dashOffset);
                }
            }

            var lineCap = style.Stroke.LineCap switch
            {
                LineCap.Flat => AM.PenLineCap.Flat,
                LineCap.Square => AM.PenLineCap.Square,
                LineCap.Round => AM.PenLineCap.Round,
                _ => throw new NotImplementedException()
            };

            var brush = ToBrush(style.Stroke.Color);
            var pen = new AM.Immutable.ImmutablePen(brush, thickness, dashStyle, lineCap);

            return pen;
        }

        public static AM.IPen ToPen(BaseColorViewModel colorViewModel, double thickness)
        {
            var dashStyle = default(AM.Immutable.ImmutableDashStyle);
            var lineCap = AM.PenLineCap.Flat;
            var brush = ToBrush(colorViewModel);
            var pen = new AM.Immutable.ImmutablePen(brush, thickness, dashStyle, lineCap);
            return pen;
        }
    }
}
