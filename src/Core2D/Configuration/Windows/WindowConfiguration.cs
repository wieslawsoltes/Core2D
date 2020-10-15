using Avalonia.Controls;

namespace Core2D.Configuration.Windows
{
    public class WindowConfiguration
    {
        public double X { get; set; } = double.NaN;
        public double Y { get; set; } = double.NaN;
        public double Width { get; set; } = double.NaN;
        public double Height { get; set; } = double.NaN;
        public WindowState WindowState { get; set; } = WindowState.Normal;
    }
}
