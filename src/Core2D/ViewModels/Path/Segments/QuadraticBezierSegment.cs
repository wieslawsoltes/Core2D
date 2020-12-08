using System;
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    public partial class QuadraticBezierSegment : PathSegment
    {
        [AutoNotify] private PointShape _point1;
        [AutoNotify] private PointShape _point2;

        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(Point1);
            points.Add(Point2);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Point1.IsDirty();
            isDirty |= Point2.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            Point1.Invalidate();
            Point2.Invalidate();
        }

        public override string ToXamlString()
            => $"Q{Point1.ToXamlString()} {Point2.ToXamlString()}";

        public override string ToSvgString()
            => $"Q{Point1.ToSvgString()} {Point2.ToSvgString()}";
    }
}
