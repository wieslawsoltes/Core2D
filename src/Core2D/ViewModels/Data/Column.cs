using System.Collections.Generic;

namespace Core2D.Data
{
    public partial class Column : ViewModelBase
    {
        [AutoNotify] private bool _isVisible;

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
