using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Style
{
    [DataContract(IsReference = true)]
    public class LineStyle : ObservableObject
    {
        private bool _isCurved;
        private double _curvature;
        private CurveOrientation _curveOrientation;
        private LineFixedLength _fixedLength;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsCurved
        {
            get => _isCurved;
            set => RaiseAndSetIfChanged(ref _isCurved, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double Curvature
        {
            get => _curvature;
            set => RaiseAndSetIfChanged(ref _curvature, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public CurveOrientation CurveOrientation
        {
            get => _curveOrientation;
            set => RaiseAndSetIfChanged(ref _curveOrientation, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public LineFixedLength FixedLength
        {
            get => _fixedLength;
            set => RaiseAndSetIfChanged(ref _fixedLength, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new LineStyle()
            {
                Name = this.Name,
                IsCurved = this.IsCurved,
                Curvature = this.Curvature,
                CurveOrientation = this.CurveOrientation,
                FixedLength = (LineFixedLength)this.FixedLength.Copy(shared)
            };
        }
    }
}
