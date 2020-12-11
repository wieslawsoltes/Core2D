using System;
using System.Collections.Generic;
using System.Globalization;
using Core2D.Model.Path;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path.Segments
{
    public partial class ArcSegmentViewModel : PathSegmentViewModel
    {
        public static SweepDirection[] SweepDirectionValues { get; } = (SweepDirection[])Enum.GetValues(typeof(SweepDirection));

        [AutoNotify] private PointShapeViewModel _point;
        [AutoNotify] private PathSizeViewModel _size;
        [AutoNotify] private double _rotationAngle;
        [AutoNotify] private bool _isLargeArc;
        [AutoNotify] private SweepDirection _sweepDirection;

        public ArcSegmentViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
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
            isDirty |= Size.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            Point.Invalidate();
            Size.Invalidate();
        }

        public override string ToXamlString()
            => $"A{Size.ToXamlString()} {RotationAngle.ToString(CultureInfo.InvariantCulture)} {(IsLargeArc ? "1" : "0")} {(SweepDirection == SweepDirection.Clockwise ? "1" : "0")} {Point.ToXamlString()}";

        public override string ToSvgString()
            => $"A{Size.ToSvgString()} {RotationAngle.ToString(CultureInfo.InvariantCulture)} {(IsLargeArc ? "1" : "0")} {(SweepDirection == SweepDirection.Clockwise ? "1" : "0")} {Point.ToSvgString()}";
    }
}
