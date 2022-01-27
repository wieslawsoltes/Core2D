#nullable enable
using System;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Layout;

public static class PointUtil
{
    public static double Snap(double value, double snap)
    {
        if (snap == 0.0)
        {
            return value;
        }
        var c = value % snap;
        var r = c >= snap / 2.0 ? value + snap - c : value - c;
        return r;
    }

    public static int CompareX(PointShapeViewModel point1, PointShapeViewModel point2)
    {
        return (point1.X > point2.X) ? 1 : ((point1.X < point2.X) ? -1 : 0);
    }

    public static int CompareY(PointShapeViewModel point1, PointShapeViewModel point2)
    {
        return (point1.Y > point2.Y) ? 1 : ((point1.Y < point2.Y) ? -1 : 0);
    }

    public static void Rotate(PointShapeViewModel point, double radians, double centerX, double centerY, out double x, out double y)
    {
        x = (point.X - centerX) * Math.Cos(radians) - (point.Y - centerY) * Math.Sin(radians) + centerX;
        y = (point.X - centerX) * Math.Sin(radians) + (point.Y - centerY) * Math.Cos(radians) + centerY;
    }
}
