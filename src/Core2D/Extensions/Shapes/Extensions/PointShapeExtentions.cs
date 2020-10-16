using static System.Math;

namespace Core2D.Shapes
{
    public static class PointShapeExtentions
    {
        public static double DistanceTo(this PointShape point, PointShape other)
        {
            double dx = point.X - other.X;
            double dy = point.Y - other.Y;
            return Sqrt(dx * dx + dy * dy);
        }
    }
}
