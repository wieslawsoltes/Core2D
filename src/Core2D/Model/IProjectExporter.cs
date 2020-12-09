using System.IO;
using Core2D.Containers;

namespace Core2D
{
    public interface IProjectExporter
    {
        void Save(Stream stream, PageContainerViewModel containerViewModel);

        void Save(Stream stream, DocumentContainerViewModel document);

        void Save(Stream stream, ProjectContainerViewModel project);
    }
}
