// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// Factory used to create new projects, documents and containers.
    /// </summary>
    public interface IProjectFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="project">The new container owner project.</param>
        /// <param name="name">The new container name.</param>
        /// <returns>The new instance of the <see cref="Container"/>.</returns>
        Container GetTemplate(Project project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="Container"/> class.
        /// </summary>
        /// <param name="project">The new container owner project.</param>
        /// <param name="name">The new container name.</param>
        /// <returns>The new instance of the <see cref="Container"/>.</returns>
        Container GetContainer(Project project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="Document"/> class.
        /// </summary>
        /// <param name="project">The new document owner project.</param>
        /// <param name="name">The new document name.</param>
        /// <returns>The new instance of the <see cref="Document"/>.</returns>
        Document GetDocument(Project project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="Project"/> class.
        /// </summary>
        /// <returns>The new instance of the <see cref="Project"/>.</returns>
        Project GetProject();
    }
}
