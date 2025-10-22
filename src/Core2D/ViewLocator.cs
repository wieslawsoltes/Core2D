// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Core2D.ViewModels;
using Dock.Model.Mvvm.Core;
using StaticViewLocator;

namespace Core2D;

[StaticViewLocator]
public partial class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
        {
            return null;
        }

        var type = data.GetType();

        if (s_views.TryGetValue(type, out var func))
        {
            return func.Invoke();
        }

        throw new Exception($"Unable to create view for type: {type}");
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase or DockableBase;
    }
}
