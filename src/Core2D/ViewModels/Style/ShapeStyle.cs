using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Style
{
    [DataContract(IsReference = true)]
    public class ShapeStyle : ViewModelBase
    {
        private StrokeStyle _stroke;
        private FillStyle _fill;
        private TextStyle _textStyle;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public StrokeStyle Stroke
        {
            get => _stroke;
            set => RaiseAndSetIfChanged(ref _stroke, value);
        }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public FillStyle Fill
        {
            get => _fill;
            set => RaiseAndSetIfChanged(ref _fill, value);
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
                Stroke = (StrokeStyle)this.Stroke.Copy(shared),
                Fill = (FillStyle)this.Fill.Copy(shared),
                TextStyle = (TextStyle)this.TextStyle.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Stroke.IsDirty();
            isDirty |= Fill.IsDirty();
            isDirty |= TextStyle.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            Stroke.Invalidate();
            Fill.Invalidate();
            TextStyle.Invalidate();
        }
    }
}
