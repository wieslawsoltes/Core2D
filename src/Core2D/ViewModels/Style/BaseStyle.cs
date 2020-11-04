using System.Runtime.Serialization;

namespace Core2D.Style
{
    [DataContract(IsReference = true)]
    public abstract class BaseStyle : ObservableObject
    {
        private BaseColor _stroke;
        private BaseColor _fill;
        private double _thickness;
        private LineCap _lineCap;
        private string _dashes;
        private double _dashOffset;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public BaseColor Stroke
        {
            get => _stroke;
            set => RaiseAndSetIfChanged(ref _stroke, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public BaseColor Fill
        {
            get => _fill;
            set => RaiseAndSetIfChanged(ref _fill, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double Thickness
        {
            get => _thickness;
            set => RaiseAndSetIfChanged(ref _thickness, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public LineCap LineCap
        {
            get => _lineCap;
            set => RaiseAndSetIfChanged(ref _lineCap, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public string Dashes
        {
            get => _dashes;
            set => RaiseAndSetIfChanged(ref _dashes, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public double DashOffset
        {
            get => _dashOffset;
            set => RaiseAndSetIfChanged(ref _dashOffset, value);
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Stroke.IsDirty();
            isDirty |= Fill.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            Stroke.Invalidate();
            Fill.Invalidate();
        }
    }
}
