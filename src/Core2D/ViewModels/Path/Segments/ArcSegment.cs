using System;
using System.Collections.Generic;
using System.Globalization;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    public class ArcSegment : PathSegment
    {
        private PointShape _point;
        private PathSize _size;
        private double _rotationAngle;
        private bool _isLargeArc;
        private SweepDirection _sweepDirection;

        public PointShape Point
        {
            get => _point;
            set => RaiseAndSetIfChanged(ref _point, value);
        }

        public PathSize Size
        {
            get => _size;
            set => RaiseAndSetIfChanged(ref _size, value);
        }

        public double RotationAngle
        {
            get => _rotationAngle;
            set => RaiseAndSetIfChanged(ref _rotationAngle, value);
        }

        public bool IsLargeArc
        {
            get => _isLargeArc;
            set => RaiseAndSetIfChanged(ref _isLargeArc, value);
        }

        public SweepDirection SweepDirection
        {
            get => _sweepDirection;
            set => RaiseAndSetIfChanged(ref _sweepDirection, value);
        }

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

        public virtual bool ShouldSerializePoint() => _point != null;

        public virtual bool ShouldSerializeSize() => _size != null;

        public virtual bool ShouldSerializeRotationAngle() => _rotationAngle != default;

        public virtual bool ShouldSerializeIsLargeArc() => _isLargeArc != default;

        public virtual bool ShouldSerializeSweepDirection() => _sweepDirection != default;
    }
}
