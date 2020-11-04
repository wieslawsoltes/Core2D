using Core2D.Containers;

namespace Core2D
{
    public interface IContainerFactory
    {
        PageContainer GetTemplate(ProjectContainer project, string name);

        PageContainer GetPage(ProjectContainer project, string name);

        DocumentContainer GetDocument(ProjectContainer project, string name);

        ProjectContainer GetProject();
    }
}
