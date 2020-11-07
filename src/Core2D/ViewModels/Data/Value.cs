using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Core2D.Data
{
    [DataContract(IsReference = true)]
    public class Value : ViewModelBase
    {
        private string _content;

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public string Content
        {
            get => _content;
            set => RaiseAndSetIfChanged(ref _content, value);
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new Value()
            {
                Name = Name,
                Content = Content
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
