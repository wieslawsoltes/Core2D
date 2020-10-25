using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    [DataContract(IsReference = true)]
    public class ArcSegment : PathSegment
    {
        private PointShape _point;
        private PathSize _size;
        private double _rotationAngle;
        private bool _isLargeArc;
        private SweepDirection _sweepDirection;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointShape Point
        {
            get => _point;
            set => RaiseAndSetIfChanged(ref _point, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PathSize Size
        {
            get => _size;
            set => RaiseAndSetIfChanged(ref _size, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double RotationAngle
        {
            get => _rotationAngle;
            set => RaiseAndSetIfChanged(ref _rotationAngle, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsLargeArc
        {
            get => _isLargeArc;
            set => RaiseAndSetIfChanged(ref _isLargeArc, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
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
    }
}
