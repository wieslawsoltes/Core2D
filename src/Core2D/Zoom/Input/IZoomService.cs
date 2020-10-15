
namespace Core2D.UI.Zoom.Input
{
    public interface IZoomService
    {
        IZoomServiceState ZoomServiceState { get; set; }
        void Wheel(double delta, double x, double y);
        void Pressed(double x, double y);
        void Released(double x, double y);
        void Moved(double x, double y);
        void Invalidate(bool redraw);
        void ZoomTo(double zoom, double x, double y);
        void ZoomDeltaTo(double delta, double x, double y);
        void StartPan(double x, double y);
        void PanTo(double x, double y);
        void Reset();
        void Center(double panelWidth, double panelHeight, double elementWidth, double elementHeight);
        void Fill(double panelWidth, double panelHeight, double elementWidth, double elementHeight);
        void Uniform(double panelWidth, double panelHeight, double elementWidth, double elementHeight);
        void UniformToFill(double panelWidth, double panelHeight, double elementWidth, double elementHeight);
        void ResetZoom(bool redraw);
        void CenterZoom(bool redraw);
        void FillZoom(bool redraw);
        void UniformZoom(bool redraw);
        void UniformToFillZoom(bool redraw);
    }
}
