#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Style
{
    public partial class BaseColorViewModel : ViewModelBase
    {
        protected BaseColorViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }
    }
}
