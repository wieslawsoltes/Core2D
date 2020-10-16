using System.IO;
using Core2D.Containers;

namespace Core2D
{
    public interface IProjectExporter
    {
        void Save(Stream stream, PageContainer container);

        void Save(Stream stream, DocumentContainer document);

        void Save(Stream stream, ProjectContainer project);
    }
}
