using System;

namespace Core2D.Style
{
    [Flags]
    public enum LineFixedLengthFlags
    {
        Disabled = 0,

        Start = 1,

        End = 2,

        Vertical = 4,

        Horizontal = 8,

        All = 16
    }
}
