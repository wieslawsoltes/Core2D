using System;
using System.Collections.Generic;

namespace Core2D.Editor.Layout
{
    public readonly struct GroupBox
    {
        public readonly IList<ShapeBox> Boxes;
        public readonly double Left;
        public readonly double Top;
        public readonly double Right;
        public readonly double Bottom;
        public readonly double CenterX;
        public readonly double CenterY;
        public readonly double Width;
        public readonly double Height;

        public GroupBox(IList<ShapeBox> boxes)
        {
            Boxes = boxes;
            Left = double.MaxValue;
            Top = double.MaxValue;
            Right = double.MinValue;
            Bottom = double.MinValue;
            foreach (var box in Boxes)
            {
                Left = Math.Min(Left, box.Left);
                Top = Math.Min(Top, box.Top);
                Right = Math.Max(Right, box.Right);
                Bottom = Math.Max(Bottom, box.Bottom);
            }
            CenterX = (Left + Right) / 2.0;
            CenterY = (Top + Bottom) / 2.0;
            Width = Math.Abs(Right - Left);
            Height = Math.Abs(Bottom - Top);
        }
    }
}
