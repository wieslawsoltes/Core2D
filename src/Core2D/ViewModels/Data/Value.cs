using System.Collections.Generic;

namespace Core2D.Data
{
    public class Value : ObservableObject
    {
        private string _content;

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

        public virtual bool ShouldSerializeContent() => !string.IsNullOrWhiteSpace(_content);
    }
}
