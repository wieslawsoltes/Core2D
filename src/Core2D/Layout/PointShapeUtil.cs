using System;
using Core2D.Shapes;

namespace Core2D.Layout
{
    internal static class PointShapeUtil
    {
        public static int CompareX(IPointShape point1, IPointShape point2)
        {
            return (point1.X > point2.X) ? 1 : ((point1.X < point2.X) ? -1 : 0);
        }

        public static int CompareY(IPointShape point1, IPointShape point2)
        {
            return (point1.Y > point2.Y) ? 1 : ((point1.Y < point2.Y) ? -1 : 0);
        }

        public static void Rotate(IPointShape point, double radians, double centerX, double centerY, out double x, out double y)
        {
            x = (point.X - centerX) * Math.Cos(radians) - (point.Y - centerY) * Math.Sin(radians) + centerX;
            y = (point.X - centerX) * Math.Sin(radians) + (point.Y - centerY) * Math.Cos(radians) + centerY;
        }
    }
}
