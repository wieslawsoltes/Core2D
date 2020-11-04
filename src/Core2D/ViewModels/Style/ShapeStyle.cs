using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Style
{
    [DataContract(IsReference = true)]
    public class ShapeStyle : BaseStyle
    {
        private ArrowStyle _startArrowStyle;
        private ArrowStyle _endArrowStyle;
        private TextStyle _textStyle;

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

            isDirty |= StartArrowStyle.IsDirty();
            isDirty |= EndArrowStyle.IsDirty();
            isDirty |= TextStyle.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            StartArrowStyle.Invalidate();
            EndArrowStyle.Invalidate();
            TextStyle.Invalidate();
        }
    }
}
