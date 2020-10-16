using System.Collections.Generic;

namespace Core2D.Data
{
    public class Property : ObservableObject
    {
        private string _value;

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

        public virtual bool ShouldSerializeValue() => !string.IsNullOrWhiteSpace(_value);
    }
}
