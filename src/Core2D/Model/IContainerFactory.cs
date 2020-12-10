using Core2D.ViewModels.Containers;

namespace Core2D.Model
{
    public interface IContainerFactory
    {
        PageContainerViewModel GetTemplate(ProjectContainerViewModel project, string name);

        PageContainerViewModel GetPage(ProjectContainerViewModel project, string name);

        DocumentContainerViewModel GetDocument(ProjectContainerViewModel project, string name);

        ProjectContainerViewModel GetProject();
    }
}
