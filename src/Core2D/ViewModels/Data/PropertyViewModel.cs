#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Data
{
    public partial class PropertyViewModel : ViewModelBase
    {
        [AutoNotify] private string? _value;

        public PropertyViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var copy = new PropertyViewModel(ServiceProvider)
            {
                Name = Name,
                Value = Value
            };

            return copy;
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();
            return isDirty;
        }

        public override string ToString() => _value ?? "";
    }
}
