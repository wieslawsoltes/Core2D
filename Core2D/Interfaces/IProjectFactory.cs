// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Defines project factory contract.
    /// </summary>
    public interface IProjectFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Template"/> class.
        /// </summary>
        /// <param name="project">The new template owner project.</param>
        /// <param name="name">The new template name.</param>
        /// <returns>The new instance of the <see cref="Template"/>.</returns>
        Template GetTemplate(Project project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="Page"/> class.
        /// </summary>
        /// <param name="project">The new page owner project.</param>
        /// <param name="name">The new page name.</param>
        /// <returns>The new instance of the <see cref="Page"/>.</returns>
        Page GetPage(Project project, string name);

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
