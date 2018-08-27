// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines text field reader contract.
    /// </summary>
    /// <typeparam name="T">The database type.</typeparam>
    public interface ITextFieldReader<T>
    {
        /// <summary>
        /// Read fields from text database file format.
        /// </summary>
        /// <param name="path">The fields file path.</param>
        /// <param name="fs">The file system.</param>
        /// <returns>The new instance of the database.</returns>
        T Read(string path, IFileSystem fs);
    }
}
