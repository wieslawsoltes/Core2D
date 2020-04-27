using System;

namespace Core2D.UI.Zoom.Input
{
    public interface IInputService
    {
        Action Capture { get; set; }
        Action Release { get; set; }
        Func<bool> IsCaptured { get; set; }
        Action Redraw { get; set; }
    }
}
