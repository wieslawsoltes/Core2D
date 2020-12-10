using System.Collections.Generic;

namespace Core2D.ViewModels.Data
{
    public partial class ValueViewModel : ViewModelBase
    {
        [AutoNotify] private string _content;

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ValueViewModel()
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
