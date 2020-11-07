using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Style
{
    [DataContract(IsReference = true)]
    public class FillStyle : ViewModelBase
    {
        private BaseColor _color;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public BaseColor Color
        {
            get => _color;
            set => RaiseAndSetIfChanged(ref _color, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new FillStyle()
            {
                Name = this.Name,
                Color = (BaseColor)this.Color.Copy(shared)
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Color.IsDirty();

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
            Color.Invalidate();
        }
    }
}
