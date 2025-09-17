#nullable enable
using Core2D.ViewModels.Containers;
using Dock.Model.Core;
using Dock.Model.Mvvm.Controls;

namespace Core2D.ViewModels.Docking.Documents;

public class PageViewModel : Document
{
    private PageContainerViewModel? _page;

    public PageContainerViewModel? Page
    {
        get => _page;
        set => SetProperty(ref _page, value);
    }
}
