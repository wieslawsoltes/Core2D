using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Style
{
    [DataContract(IsReference = true)]
    public class ShapeStyle : ViewModelBase
    {
        private BaseColor _stroke;
        private BaseColor _fill;
        private double _thickness;
        private LineCap _lineCap;
        private string _dashes;
        private double _dashOffset;
        private ArrowStyle _startArrowStyle;
        private ArrowStyle _endArrowStyle;
        private TextStyle _textStyle;

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

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public TextStyle TextStyle
        {
            get => _textStyle;
            set => RaiseAndSetIfChanged(ref _textStyle, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ShapeStyle()
            {
                Name = this.Name,
                Stroke = (BaseColor)this.Stroke.Copy(shared),
                Fill = (BaseColor)this.Fill.Copy(shared),
                Thickness = this.Thickness,
                LineCap = this.LineCap,
                Dashes = this.Dashes,
                DashOffset = this.DashOffset,
                TextStyle = (TextStyle)this.TextStyle.Copy(shared),
                StartArrowStyle = (ArrowStyle)this.StartArrowStyle.Copy(shared),
                EndArrowStyle = (ArrowStyle)this.EndArrowStyle.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Stroke.IsDirty();
            isDirty |= Fill.IsDirty();
            isDirty |= StartArrowStyle.IsDirty();
            isDirty |= EndArrowStyle.IsDirty();
            isDirty |= TextStyle.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            Stroke.Invalidate();
            Fill.Invalidate();
            StartArrowStyle.Invalidate();
            EndArrowStyle.Invalidate();
            TextStyle.Invalidate();
        }
    }
}
