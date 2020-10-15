
namespace Core2D.UI.Zoom.Input
{
    public interface IZoomServiceState
    {
        double ZoomSpeed { get; set; }
        double ZoomX { get; set; }
        double ZoomY { get; set; }
        double OffsetX { get; set; }
        double OffsetY { get; set; }
        bool IsPanning { get; set; }
        bool IsZooming { get; set; }
        FitMode InitFitMode { get; set; }
        FitMode AutoFitMode { get; set; }
    }
}
