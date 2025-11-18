// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Core2D.ViewModels;
using Dock.Model.Mvvm.Core;
using StaticViewLocator;

namespace Core2D;

[StaticViewLocator]
public partial class ViewLocator : IDataTemplate
{
    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "View fallback uses reflection to locate views by name.")]
    [UnconditionalSuppressMessage("Trimming", "IL2072", Justification = "Fallback assumes views expose public parameterless constructors.")]
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

        // Fallback to convention-based lookup when generator did not produce a mapping
        var viewName = type.FullName?
            .Replace(".ViewModels.", ".Views.")
            .Replace("ViewModel", "View");

        if (!string.IsNullOrWhiteSpace(viewName))
        {
            var viewType = typeof(ViewLocator).Assembly.GetType(viewName!);
            if (viewType is not null &&
                Activator.CreateInstance(viewType) is Control control)
            {
                return control;
            }
        }

        throw new Exception($"Unable to create view for type: {type}");
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase or DockableBase;
    }
}
