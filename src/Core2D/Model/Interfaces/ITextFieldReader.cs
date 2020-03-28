using System.IO;

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines text field reader contract.
    /// </summary>
    /// <typeparam name="T">The database type.</typeparam>
    public interface ITextFieldReader<T>
    {
        /// <summary>
        /// Gets text field reader name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets text field reader extension.
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Read fields from text database file format.
        /// </summary>
        /// <param name="stream">The fields file stream.</param>
        /// <returns>The new instance of the database.</returns>
        T Read(Stream stream);
    }
}
