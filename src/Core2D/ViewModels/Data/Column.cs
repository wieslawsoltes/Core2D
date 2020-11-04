using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Data
{
    [DataContract(IsReference = true)]
    public class Column : ObservableObject
    {
        private bool _isVisible;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsVisible
        {
            get => _isVisible;
            set => RaiseAndSetIfChanged(ref _isVisible, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new Column()
            {
                Name = this.Name,
                IsVisible = this.IsVisible
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
    }
}
