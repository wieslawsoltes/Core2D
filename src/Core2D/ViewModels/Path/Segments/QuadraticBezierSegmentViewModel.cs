using System;
using System.Collections.Generic;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path.Segments
{
    public partial class QuadraticBezierSegmentViewModel : PathSegmentViewModel
    {
        [AutoNotify] private PointShapeViewModel _point1;
        [AutoNotify] private PointShapeViewModel _point2;

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            points.Add(Point1);
            points.Add(Point2);
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
