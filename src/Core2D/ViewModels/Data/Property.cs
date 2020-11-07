using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Data
{
    [DataContract(IsReference = true)]
    public class Property : ViewModelBase
    {
        private string _value;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public string Value
        {
            get => _value;
            set => RaiseAndSetIfChanged(ref _value, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new Property()
            {
                Name = Name,
                Value = Value
            };
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();
        }

        public override string ToString() => _value.ToString();
    }
}
