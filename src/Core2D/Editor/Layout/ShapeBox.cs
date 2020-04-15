using System;
using System.Collections.Generic;
using Core2D.History;
using Core2D.Shapes;

namespace Core2D.Editor.Layout
{
    public struct ShapeBox
    {
        public readonly IBaseShape Shape;
        public readonly List<IPointShape> Points;
        public Box Bounds;

        public static int CompareLeft(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Bounds.Left > box2.Bounds.Left) ? 1 : ((box1.Bounds.Left < box2.Bounds.Left) ? -1 : 0);
        }

        public static int CompareRight(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Bounds.Right > box2.Bounds.Right) ? 1 : ((box1.Bounds.Right < box2.Bounds.Right) ? -1 : 0);
        }

        public static int CompareTop(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Bounds.Top > box2.Bounds.Top) ? 1 : ((box1.Bounds.Top < box2.Bounds.Top) ? -1 : 0);
        }

        public static int CompareBottom(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Bounds.Bottom > box2.Bounds.Bottom) ? 1 : ((box1.Bounds.Bottom < box2.Bounds.Bottom) ? -1 : 0);
        }

        public static int CompareCenterX(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Bounds.CenterX > box2.Bounds.CenterX) ? 1 : ((box1.Bounds.CenterX < box2.Bounds.CenterX) ? -1 : 0);
        }

        public static int CompareCenterY(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Bounds.CenterY > box2.Bounds.CenterY) ? 1 : ((box1.Bounds.CenterY < box2.Bounds.CenterY) ? -1 : 0);
        }

        public static int CompareWidth(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Bounds.Width > box2.Bounds.Width) ? 1 : ((box1.Bounds.Width < box2.Bounds.Width) ? -1 : 0);
        }

        public static int CompareHeight(ShapeBox box1, ShapeBox box2)
        {
            return (box1.Bounds.Height > box2.Bounds.Height) ? 1 : ((box1.Bounds.Height < box2.Bounds.Height) ? -1 : 0);
        }

        public static void Rotate(IPointShape point, double radians, double centerX, double centerY, out double x, out double y)
        {
            x = (point.X - centerX) * Math.Cos(radians) - (point.Y - centerY) * Math.Sin(radians) + centerX;
            y = (point.X - centerX) * Math.Sin(radians) + (point.Y - centerY) * Math.Cos(radians) + centerY;
        }

        public ShapeBox(IBaseShape shape)
        {
            Shape = shape;

            Points = new List<IPointShape>();

            foreach (var point in Shape.GetPoints())
            {
                Points.Add(point);
            }

            Bounds = new Box();

            Update(ref Bounds);
        }

        public void Update(ref Box bounds)
        {
            var left = double.MaxValue;
            var top = double.MaxValue;
            var right = double.MinValue;
            var bottom = double.MinValue;

            for (int i = 0; i < Points.Count; i++)
            {
                var point = Points[i];
                left = Math.Min(left, point.X);
                top = Math.Min(top, point.Y);
                right = Math.Max(right, point.X);
                bottom = Math.Max(bottom, point.Y);
            }

            bounds.Left = left;
            bounds.Top = top;
            bounds.Right = right;
            bounds.Bottom = bottom;
            bounds.CenterX = (bounds.Left + bounds.Right) / 2.0;
            bounds.CenterY = (bounds.Top + bounds.Bottom) / 2.0;
            bounds.Width = Math.Abs(bounds.Right - bounds.Left);
            bounds.Height = Math.Abs(bounds.Bottom - bounds.Top);
        }

        public void MoveByWithHistory(double dx, double dy, IHistory history)
        {
            var previous = new { DeltaX = -dx, DeltaY = -dy, Shape };
            var next = new { DeltaX = dx, DeltaY = dy, Shape };
            history.Snapshot(previous, next, (s) => s.Shape.Move(null, s.DeltaX, s.DeltaY));
            Shape.Move(null, dx, dy);
        }
    }
}
