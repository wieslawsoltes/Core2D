using System.IO;

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines text field writer contract.
    /// </summary>
    /// <typeparam name="T">The database type.</typeparam>
    public interface ITextFieldWriter<T>
    {
        /// <summary>
        /// Gets text field writer name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets text field writer extension.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Write database records to text based file format.
        /// </summary>
        /// <param name="stream,">The fields file stream,.</param>
        /// <param name="database">The source records database.</param>
        void Write(Stream stream, T database);
    }
}
