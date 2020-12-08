using System.Collections.Generic;

namespace Core2D.Data
{
    public partial class Value : ViewModelBase
    {
        [AutoNotify] private string _content;

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
