// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers;

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines project exporter contract.
    /// </summary>
    public interface IProjectExporter
    {
        /// <summary>
        /// Save <see cref="PageContainer"/> to a file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="container">The container instance.</param>
        void Save(string path, PageContainer container);

        /// <summary>
        /// Save <see cref="DocumentContainer"/> to a file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="document">The document instance.</param>
        void Save(string path, DocumentContainer document);

        /// <summary>
        /// Save <see cref="ProjectContainer"/> to a file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="project">The project instance.</param>
        void Save(string path, ProjectContainer project);
    }
}
