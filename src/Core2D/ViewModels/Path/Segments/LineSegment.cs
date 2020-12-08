using System;
using System.Collections.Generic;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    public partial class LineSegment : PathSegment
    {
        [AutoNotify] private PointShape _point;

        public override void GetPoints(IList<PointShape> points)
        {
            points.Add(Point);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Point.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            Point.Invalidate();
        }

        public override string ToXamlString()
            => $"L{Point.ToXamlString()}";

        public override string ToSvgString()
            => $"L{Point.ToSvgString()}";
    }
}
