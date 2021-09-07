#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Scripting
{
    public partial class ScriptViewModel : ViewModelBase
    {
        [AutoNotify] private string? _code;

        public ScriptViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var copy = new ScriptViewModel(ServiceProvider)
            {
                Code = _code
            };

            return copy;
        }
    }
}
