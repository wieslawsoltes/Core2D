// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines text field writer contract.
    /// </summary>
    /// <typeparam name="T">The database type.</typeparam>
    public interface ITextFieldWriter<T>
    {
        /// <summary>
        /// Write database records to text based file format.
        /// </summary>
        /// <param name="path">The fields file path.</param>
        /// <param name="fs">The file system.</param>
        /// <param name="database">The source records database.</param>
        void Write(string path, IFileSystem fs, T database);
    }
}
