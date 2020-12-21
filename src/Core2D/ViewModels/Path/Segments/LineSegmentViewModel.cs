#nullable disable
using System;
using System.Collections.Generic;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path.Segments
{
    public partial class LineSegmentViewModel : PathSegmentViewModel
    {
        [AutoNotify] private PointShapeViewModel _point;

        public LineSegmentViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override void GetPoints(IList<PointShapeViewModel> points)
        {
            points.Add(Point);
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
