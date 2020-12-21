#nullable disable
using System.Collections.Generic;
using System.Linq;
using Core2D.ViewModels.Shapes;
using Spatial;
using Spatial.ConvexHull;
using Spatial.Sat;

namespace Core2D.ViewModels.Editor.Bounds
{
    public static class HitTestHelper
    {
        public static MonotoneChain MC => new MonotoneChain();
        public static SeparatingAxisTheorem SAT => new SeparatingAxisTheorem();

        public static Vector2[] ToSelection(Rect2 rect)
        {
            return new Vector2[]
            {
                new Vector2(rect.X, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y),
                new Vector2(rect.X + rect.Width, rect.Y + rect.Height),
                new Vector2(rect.X, rect.Y + rect.Height)
            };
        }

        public static void ToConvexHull(IEnumerable<PointShapeViewModel> points, double scale, out int k, out Vector2[] convexHull)
        {
            Vector2[] vertices = new Vector2[points.Count()];
            int i = 0;
            foreach (var point in points)
            {
                vertices[i] = new Vector2(point.X / scale, point.Y / scale);
                i++;
            }
            MC.ConvexHull(vertices, out convexHull, out k);
        }

        public static bool Contains(IEnumerable<PointShapeViewModel> points, Point2 point, double scale)
        {
            ToConvexHull(points, scale, out int k, out var convexHull);
            bool contains = false;
            for (int i = 0, j = k - 2; i < k - 1; j = i++)
            {
                if (((convexHull[i].Y > point.Y) != (convexHull[j].Y > point.Y))
                    && (point.X < (convexHull[j].X - convexHull[i].X) * (point.Y - convexHull[i].Y) / (convexHull[j].Y - convexHull[i].Y) + convexHull[i].X))
                {
                    contains = !contains;
                }
            }
            return contains;
        }

        public static bool Overlap(IEnumerable<PointShapeViewModel> points, Vector2[] selection, double scale)
        {
            ToConvexHull(points, scale, out int k, out var convexHull);
            var vertices = convexHull.Take(k).ToArray();
            return SAT.Overlap(selection, vertices);
        }

        public static bool Overlap(IEnumerable<PointShapeViewModel> points, Rect2 rect, double scale)
        {
            return Overlap(points, ToSelection(rect), scale);
        }

        public static Rect2 Inflate(ref Rect2 rect, double scale)
        {
            double width = rect.Width / scale;
            double height = rect.Height / scale;
            Point2 center = rect.Center;
            return new Rect2(center.X - width / 2.0, center.Y - height / 2.0, width, height);
        }
    }
}
