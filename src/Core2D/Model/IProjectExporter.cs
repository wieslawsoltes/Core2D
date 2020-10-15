using System.IO;
using Core2D.Containers;

namespace Core2D
{
    /// <summary>
    /// Defines project exporter contract.
    /// </summary>
    public interface IProjectExporter
    {
        /// <summary>
        /// Save <see cref="PageContainer"/> to a stream.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="container">The container instance.</param>
        void Save(Stream stream, PageContainer container);

        /// <summary>
        /// Save <see cref="DocumentContainer"/> to a stream.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="document">The document instance.</param>
        void Save(Stream stream, DocumentContainer document);

        /// <summary>
        /// Save <see cref="ProjectContainer"/> to a stream.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="project">The project instance.</param>
        void Save(Stream stream, ProjectContainer project);
    }
}
