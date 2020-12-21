#nullable disable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Data
{
    public partial class ColumnViewModel : ViewModelBase
    {
        [AutoNotify] private bool _isVisible;

        public ColumnViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            return new ColumnViewModel(_serviceProvider)
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
