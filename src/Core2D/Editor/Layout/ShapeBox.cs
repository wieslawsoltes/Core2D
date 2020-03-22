using System;
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Editor.Layout
{
    public readonly struct ShapeBox
    {
        public readonly IBaseShape Shape;
        public readonly IEnumerable<IPointShape> Points;
        public readonly double Left;
        public readonly double Top;
        public readonly double Right;
        public readonly double Bottom;
        public readonly double CenterX;
        public readonly double CenterY;
        public readonly double Width;
        public readonly double Height;

        public static int CompareLeft(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Left > box2.Left) ? 1 : ((box1.Left < box2.Left) ? -1 : 0);
        }

        public static int CompareRight(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Right > box2.Right) ? 1 : ((box1.Right < box2.Right) ? -1 : 0);
        }

        public static int CompareTop(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Top > box2.Top) ? 1 : ((box1.Top < box2.Top) ? -1 : 0);
        }

        public static int CompareBottom(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Bottom > box2.Bottom) ? 1 : ((box1.Bottom < box2.Bottom) ? -1 : 0);
        }

        public static int CompareCenterX(ShapeBox box1, ShapeBox box2)
        {
            return (box1.CenterX > box2.CenterX) ? 1 : ((box1.CenterX < box2.CenterX) ? -1 : 0);
        }

        public static int CompareCenterY(ShapeBox box1, ShapeBox box2)
        {
            return (box1.CenterY > box2.CenterY) ? 1 : ((box1.CenterY < box2.CenterY) ? -1 : 0);
        }

        public static int CompareWidth(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Width > box2.Width) ? 1 : ((box1.Width < box2.Width) ? -1 : 0);
        }

        public static int CompareHeight(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Height > box2.Height) ? 1 : ((box1.Height < box2.Height) ? -1 : 0);
        }

        public static void GetBox(IEnumerable<IPointShape> points, out double left, out double top, out double right, out double bottom)
        {
            left = double.MaxValue;
            top = double.MaxValue;
            right = double.MinValue;
            bottom = double.MinValue;

            foreach (var point in points)
            {
                left = Math.Min(left, point.X);
                top = Math.Min(top, point.Y);
                right = Math.Max(right, point.X);
                bottom = Math.Max(bottom, point.Y);
            }
        }

        public ShapeBox(IBaseShape shape)
        {
            Shape = shape;
            Points = Shape.GetPoints();
            GetBox(Points, out Left, out Top, out Right, out Bottom);
            CenterX = (Left + Right) / 2.0;
            CenterY = (Top + Bottom) / 2.0;
            Width = Math.Abs(Right - Left);
            Height = Math.Abs(Bottom - Top);
        }
    }
}
