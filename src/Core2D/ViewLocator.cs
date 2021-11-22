#nullable enable
using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Core2D.ViewModels;
using Dock.Model.ReactiveUI.Core;

namespace Core2D;

public partial class ViewLocator : IDataTemplate
{
    public IControl Build(object data)
    {
        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        else
        {
            return new TextBlock { Text = "Not Found: " + name };
        }
    }

    public bool Match(object data)
    {
        return data is ViewModelBase || data is DockableBase;
    }
}
