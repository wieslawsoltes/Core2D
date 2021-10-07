#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Containers;

public abstract partial class BaseContainerViewModel : ViewModelBase
{
    [AutoNotify] private bool _isVisible;
    [AutoNotify] private bool _isExpanded;

    protected BaseContainerViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        _isVisible = true;
        _isExpanded = false;
    }
}