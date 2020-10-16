using System;

namespace Core2D.Renderer
{
    [Flags]
    public enum ShapeStateFlags
    {
        Default = 0,

        Visible = 1,

        Printable = 2,

        Locked = 4,

        Size = 8,

        Thickness = 16,

        Connector = 32,

        None = 64,

        Standalone = 128,

        Input = 256,

        Output = 512
    }
}
