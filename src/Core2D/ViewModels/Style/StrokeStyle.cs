using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Style
{
    [DataContract(IsReference = true)]
    public class StrokeStyle : ViewModelBase
    {
        private BaseColor _color;
        private double _thickness;
        private LineCap _lineCap;
        private string _dashes;
        private double _dashOffset;
        private ArrowStyle _startArrowStyle;
        private ArrowStyle _endArrowStyle;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public BaseColor Color
        {
            get => _color;
            set => RaiseAndSetIfChanged(ref _color, value);
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
        
        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ArrowStyle StartArrowStyle
        {
            get => _startArrowStyle;
            set => RaiseAndSetIfChanged(ref _startArrowStyle, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public ArrowStyle EndArrowStyle
        {
            get => _endArrowStyle;
            set => RaiseAndSetIfChanged(ref _endArrowStyle, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new StrokeStyle()
            {
                Name = this.Name,
                Color = (BaseColor)this.Color.Copy(shared),
                Thickness = this.Thickness,
                LineCap = this.LineCap,
                Dashes = this.Dashes,
                DashOffset = this.DashOffset,
                StartArrowStyle = (ArrowStyle)this.StartArrowStyle.Copy(shared),
                EndArrowStyle = (ArrowStyle)this.EndArrowStyle.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Color.IsDirty();
            isDirty |= StartArrowStyle.IsDirty();
            isDirty |= EndArrowStyle.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            Color.Invalidate();
            StartArrowStyle.Invalidate();
            EndArrowStyle.Invalidate();
        }
    }
}
