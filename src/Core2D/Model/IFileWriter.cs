using System.IO;

namespace Core2D
{
    /// <summary>
    /// Defines file writer contract.
    /// </summary>
    public interface IFileWriter
    {
        /// <summary>
        /// Gets file writer name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets file writer extension.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Save object item to a stream.
        /// </summary>
        /// <param name="stream">The file stream.</param>
        /// <param name="item">The object item.</param>
        /// <param name="options">The options object.</param>
        void Save(Stream stream, object item, object options);
    }
}
