// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;

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
