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
        /// Save <see cref="IPageContainer"/> to a stream.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="container">The container instance.</param>
        void Save(Stream stream, IPageContainer container);

        /// <summary>
        /// Save <see cref="IDocumentContainer"/> to a stream.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="document">The document instance.</param>
        void Save(Stream stream, IDocumentContainer document);

        /// <summary>
        /// Save <see cref="IProjectContainer"/> to a stream.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="project">The project instance.</param>
        void Save(Stream stream, IProjectContainer project);
    }
}
