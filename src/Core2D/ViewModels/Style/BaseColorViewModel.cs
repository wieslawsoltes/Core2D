#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Style;

public abstract partial class BaseColorViewModel : ViewModelBase
{
    protected BaseColorViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }
}