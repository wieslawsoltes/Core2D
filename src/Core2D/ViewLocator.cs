#nullable enable
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Core2D.ViewModels;
using Dock.Model.ReactiveUI.Core;

namespace Core2D;

[StaticViewLocator]
public partial class ViewLocator : IDataTemplate
{
    public IControl Build(object data)
    {
        var type = data.GetType();
        return s_views.TryGetValue(type, out var func) 
            ? func.Invoke() 
            : new TextBlock { Text = $"Unable to create view for type: {type}" };
    }

    public bool Match(object data)
    {
        return data is ViewModelBase or DockableBase;
    }
}
