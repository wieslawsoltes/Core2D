// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Interfaces
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
        /// Save object item to a file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="item">The object item.</param>
        /// <param name="options">The options object.</param>
        void Save(string path, object item, object options);
    }
}
