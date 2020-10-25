using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Core2D.Shapes;

namespace Core2D.Path.Segments
{
    [DataContract(IsReference = true)]
    public class LineSegment : PathSegment
    {
        private PointShape _point;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public PointShape Point
        {
            get => _point;
            set => RaiseAndSetIfChanged(ref _point, value);
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
