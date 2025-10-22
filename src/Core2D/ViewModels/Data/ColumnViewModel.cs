// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;

namespace Core2D.ViewModels.Data;

public partial class ColumnViewModel : ViewModelBase
{
    [AutoNotify] private bool _isVisible;

    public ColumnViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        var copy = new ColumnViewModel(ServiceProvider)
        {
            Name = Name,
            IsVisible = IsVisible
        };

        return copy;
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();
        return isDirty;
    }
}
