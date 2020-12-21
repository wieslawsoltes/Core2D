#nullable disable
using System;
using System.Collections.Generic;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path.Segments
{
    public partial class CubicBezierSegmentViewModel : PathSegmentViewModel
    {
        [AutoNotify] private PointShapeViewModel _point1;
        [AutoNotify] private PointShapeViewModel _point2;
        [AutoNotify] private PointShapeViewModel _point3;

        public CubicBezierSegmentViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            points.Add(Point1);
            points.Add(Point2);
            points.Add(Point3);
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Point1.IsDirty();
            isDirty |= Point2.IsDirty();
            isDirty |= Point3.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            Point1.Invalidate();
            Point2.Invalidate();
            Point3.Invalidate();
        }

        public override string ToXamlString()
            => $"C{Point1.ToXamlString()} {Point2.ToXamlString()} {Point3.ToXamlString()}";

        public override string ToSvgString()
            => $"C{Point1.ToSvgString()} {Point2.ToSvgString()} {Point3.ToSvgString()}";
    }
}
