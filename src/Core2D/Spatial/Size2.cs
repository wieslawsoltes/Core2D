using System;

namespace Core2D.Spatial
{
    public struct Size2
    {
        public readonly double Width;
        public readonly double Height;

        public Size2(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        public void Deconstruct(out double width, out double height)
        {
            width = this.Width;
            height = this.Height;
        }
    }
}
