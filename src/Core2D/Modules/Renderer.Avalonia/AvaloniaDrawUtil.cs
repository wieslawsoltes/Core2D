using System;
using Core2D.Style;
using AM = Avalonia.Media;

namespace Core2D.Renderer
{
    internal static class AvaloniaDrawUtil
    {
        public static AM.Color ToColor(ArgbColorViewModelViewModel argbColorViewModelViewModel)
        {
            return AM.Color.FromArgb(argbColorViewModelViewModel.A, argbColorViewModelViewModel.R, argbColorViewModelViewModel.G, argbColorViewModelViewModel.B);
        }

        public static AM.IBrush ToBrush(BaseColorViewModel colorViewModel) => colorViewModel switch
        {
            ArgbColorViewModelViewModel argbColor => new AM.Immutable.ImmutableSolidColorBrush(ToColor(argbColor)),
            _ => throw new NotSupportedException($"The {colorViewModel.GetType()} color type is not supported.")
        };

        public static AM.IPen ToPen(ShapeStyleViewModel styleViewModel, double thickness)
        {
            var dashStyle = default(AM.Immutable.ImmutableDashStyle);
            if (styleViewModel.Stroke.Dashes != null)
            {
                var dashes = StyleHelper.ConvertDashesToDoubleArray(styleViewModel.Stroke.Dashes, 1.0);
                var dashOffset = styleViewModel.Stroke.DashOffset;
                if (dashes != null)
                {
                    dashStyle = new AM.Immutable.ImmutableDashStyle(dashes, dashOffset);
                }
            }

            var lineCap = styleViewModel.Stroke.LineCap switch
            {
                LineCap.Flat => AM.PenLineCap.Flat,
                LineCap.Square => AM.PenLineCap.Square,
                LineCap.Round => AM.PenLineCap.Round,
                _ => throw new NotImplementedException()
            };

            var brush = ToBrush(styleViewModel.Stroke.ColorViewModel);
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
