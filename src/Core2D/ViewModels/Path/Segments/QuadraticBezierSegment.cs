using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    [DataContract(IsReference = true)]
    public class QuadraticBezierSegment : PathSegment
    {
        private PointShape _point1;
        private PointShape _point2;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointShape Point1
        {
            get => _point1;
            set => RaiseAndSetIfChanged(ref _point1, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointShape Point2
        {
            get => _point2;
            set => RaiseAndSetIfChanged(ref _point2, value);
        }

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
