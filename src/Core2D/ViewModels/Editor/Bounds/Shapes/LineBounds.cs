using System;
using System.Collections.Generic;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Shapes;
using Spatial;

namespace Core2D.ViewModels.Editor.Bounds.Shapes
{
    public partial class LineBounds : IBounds
    {
        public Type TargetType => typeof(LineShapeViewModel);

        public PointShapeViewModel TryToGetPoint(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is LineShapeViewModel line))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            var pointHitTest = registered[typeof(PointShapeViewModel)];

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

        public bool Contains(BaseShapeViewModel shape, Point2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is LineShapeViewModel line))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            Point2 a;
            Point2 b;
            if (line.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
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

        public bool Overlaps(BaseShapeViewModel shape, Rect2 target, double radius, double scale, IDictionary<Type, IBounds> registered)
        {
            if (!(shape is LineShapeViewModel line))
            {
                throw new ArgumentNullException(nameof(shape));
            }

            Point2 a;
            Point2 b;
            if (line.State.HasFlag(ShapeStateFlags.Size) && scale != 1.0)
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
