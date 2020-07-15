using System;
using System.Collections.Generic;
using Core2D.Renderer;
using Core2D.Shapes;
using DocumentFormat.OpenXml.Drawing;
using Spatial;

namespace Core2D.Editor.Bounds.Shapes
{
    public class BoundsLine : IBounds
    {
        public Type TargetType => typeof(ILineShape);

        public IPointShape TryToGetPoint(IBaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is ILineShape line))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(IPointShape)];

            if (pointHitTest.TryToGetPoint(line.Start, target, radius, scale, registered) != null)
            {
                return line.Start;
            }

            if (pointHitTest.TryToGetPoint(line.End, target, radius, scale, registered) != null)
            {
                return line.End;
            }

            return null;
        }

        public bool Contains(IBaseShape shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is ILineShape line))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            Point2 a;
            Point2 b;
            if (line.State.Flags.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
            {
                a = new Point2(line.Start.X * scale, line.Start.Y * scale);
                b = new Point2(line.End.X * scale, line.End.Y * scale);
            }
            else
            {
                a = new Point2(line.Start.X, line.Start.Y);
                b = new Point2(line.End.X, line.End.Y);
            }

            var nearest = target.NearestOnLine(a, b);
            double distance = target.DistanceTo(nearest);
            return distance < radius;
        }

        public bool Overlaps(IBaseShape shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is ILineShape line))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            Point2 a;
            Point2 b;
            if (line.State.Flags.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
            {
                a = new Point2(line.Start.X * scale, line.Start.Y * scale);
                b = new Point2(line.End.X * scale, line.End.Y * scale);
            }
            else
            {
                a = new Point2(line.Start.X, line.Start.Y);
                b = new Point2(line.End.X, line.End.Y);
            }

            return Line2.LineIntersectsWithRect(a, b, target, out double _, out double _, out double _, out double _);
        }
    }
}
