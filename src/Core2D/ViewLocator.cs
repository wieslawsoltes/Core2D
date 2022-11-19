#nullable enable
using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Core2D.ViewModels;
using Dock.Model.Mvvm.Core;

namespace Core2D;

public partial class ViewLocator : IDataTemplate
{
    public IControl Build(object? data)
    {
        var name = data?.GetType().FullName?.Replace("ViewModel", "View");
        var type = name is null ? null : Type.GetType(name);
        if (type != null)
        {
            try
            {
                var instance = Activator.CreateInstance(type);
                if (instance is Control control)
                {
                    return control;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase or DockableBase;
    }
}
