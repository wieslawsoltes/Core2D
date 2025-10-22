// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

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
