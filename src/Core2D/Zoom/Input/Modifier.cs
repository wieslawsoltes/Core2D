using System;

namespace Core2D.Zoom.Input
{
    [Flags]
    public enum Modifier
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4
    }
}
