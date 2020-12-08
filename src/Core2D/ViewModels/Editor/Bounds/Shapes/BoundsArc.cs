using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class BoundsArc : IBounds
    {
        public Type TargetType => typeof(ArcShape);

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is ArcShape arc))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(PointShape)];

            if (pointHitTest.TryToGetPoint(arc.Point1, target, radius, scale, registered) != null)
            {
                return arc.Point1;
            }

            if (pointHitTest.TryToGetPoint(arc.Point2, target, radius, scale, registered) != null)
            {
                return arc.Point2;
            }

            if (pointHitTest.TryToGetPoint(arc.Point3, target, radius, scale, registered) != null)
            {
                return arc.Point3;
            }

            if (pointHitTest.TryToGetPoint(arc.Point4, target, radius, scale, registered) != null)
            {
                return arc.Point4;
            }

            return null;
        }

        public bool Contains(BaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is ArcShape arc))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var rect = ArcBounds(arc);

            if (arc.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
            {
                return HitTestHelper.Inflate(ref rect, scale).Contains(target);
            }
            else
            {
                return rect.Contains(target);
            }
        }

        public bool Overlaps(BaseShape shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is ArcShape arc))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var rect = ArcBounds(arc);

            if (arc.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
            {
                return HitTestHelper.Inflate(ref rect, scale).IntersectsWith(target);
            }
            else
            {
                return rect.IntersectsWith(target);
            }
        }

        public static Rect2 ArcBounds(ArcShape arc)
        {
            double x1 = arc.Point1.X;
            double y1 = arc.Point1.Y;
            double x2 = arc.Point2.X;
            double y2 = arc.Point2.Y;

            double x0 = (x1 + x2) / 2.0;
            double y0 = (y1 + y2) / 2.0;

            double r = Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
            double x = x0 - r;
            double y = y0 - r;
            double width = 2.0 * r;
            double height = 2.0 * r;

            return new Rect2(x, y, width, height);
        }
    }
}
