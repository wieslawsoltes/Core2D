// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Project;
using Core2D.Renderer;

namespace Core2D.Interfaces
{
    /// <summary>
    /// Defines project exporter contract.
    /// </summary>
    public interface IProjectExporter
    {
        /// <summary>
        /// Save <see cref="XContainer"/> to a file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="container">The container instance.</param>
        /// <param name="renderer">The shape renderer.</param>
        void Save(string path, XContainer container, ShapeRenderer renderer);

        /// <summary>
        /// Save <see cref="XDocument"/> to a file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="document">The project instance.</param>
        /// <param name="renderer">The shape renderer.</param>
        void Save(string path, XDocument document, ShapeRenderer renderer);

        /// <summary>
        /// Save <see cref="XProject"/> to a file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="project">The project instance.</param>
        /// <param name="renderer">The shape renderer.</param>
        void Save(string path, XProject project, ShapeRenderer renderer);
    }
}
