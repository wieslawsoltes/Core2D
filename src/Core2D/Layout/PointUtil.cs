using System;
using Core2D.Shapes;

namespace Core2D.Layout
{
    internal static class PointUtil
    {
        public static decimal Snap(decimal value, decimal snap)
        {
            if (snap == 0m)
            {
                return value;
            }
            decimal c = value % snap;
            decimal r = c >= snap / 2m ? value + snap - c : value - c;
            return r;
        }

        public static int CompareX(IPointShape point1, IPointShape point2)
        {
            return (point1.X > point2.X) ? 1 : ((point1.X < point2.X) ? -1 : 0);
        }

        public static int CompareY(IPointShape point1, IPointShape point2)
        {
            return (point1.Y > point2.Y) ? 1 : ((point1.Y < point2.Y) ? -1 : 0);
        }

        public static void Rotate(IPointShape point, decimal radians, decimal centerX, decimal centerY, out decimal x, out decimal y)
        {
            x = ((decimal)point.X - centerX) * (decimal)Math.Cos((double)radians) - ((decimal)point.Y - centerY) * (decimal)Math.Sin((double)radians) + centerX;
            y = ((decimal)point.X - centerX) * (decimal)Math.Sin((double)radians) + ((decimal)point.Y - centerY) * (decimal)Math.Cos((double)radians) + centerY;
        }
    }
}
