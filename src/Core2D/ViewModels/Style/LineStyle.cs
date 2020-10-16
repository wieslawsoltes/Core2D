using System.Collections.Generic;

namespace Core2D.Style
{
    public class LineStyle : ObservableObject
    {
        private bool _isCurved;
        private double _curvature;
        private CurveOrientation _curveOrientation;
        private LineFixedLength _fixedLength;

        public bool IsCurved
        {
            get => _isCurved;
            set => RaiseAndSetIfChanged(ref _isCurved, value);
        }

        public double Curvature
        {
            get => _curvature;
            set => RaiseAndSetIfChanged(ref _curvature, value);
        }

        public CurveOrientation CurveOrientation
        {
            get => _curveOrientation;
            set => RaiseAndSetIfChanged(ref _curveOrientation, value);
        }

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

        public virtual bool ShouldSerializeIsCurved() => _isCurved != default;

        public virtual bool ShouldSerializeCurvature() => _curvature != default;

        public virtual bool ShouldSerializeCurveOrientation() => _curveOrientation != default;

        public virtual bool ShouldSerializeFixedLength() => _fixedLength != null;
    }
}
