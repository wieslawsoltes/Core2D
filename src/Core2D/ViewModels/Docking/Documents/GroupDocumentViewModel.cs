#nullable enable
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Dock.Model.Mvvm.Controls;

namespace Core2D.ViewModels.Docking.Documents;

public class GroupDocumentViewModel : Document
{
    private GroupShapeViewModel? _group;
    private PageContainerViewModel? _container;

    public GroupShapeViewModel? Group
    {
        get => _group;
        set => SetProperty(ref _group, value);
    }

    public PageContainerViewModel? Container
    {
        get => _container;
        set => SetProperty(ref _container, value);
    }
}
