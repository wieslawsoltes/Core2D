using System;

namespace Core2D.ViewModels.Containers
{
    public partial class BaseContainerViewModel : ViewModelBase
    {
        [AutoNotify] private bool _isVisible;
        [AutoNotify] private bool _isExpanded;

        public BaseContainerViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _isVisible = true;
            _isExpanded = true;
        }
    }
}
