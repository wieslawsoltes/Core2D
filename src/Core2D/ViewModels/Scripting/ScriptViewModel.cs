#nullable disable
using System;

namespace Core2D.ViewModels.Scripting
{
    public partial class ScriptViewModel : ViewModelBase
    {
        [AutoNotify] private string _code;

        public ScriptViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
    }
}
