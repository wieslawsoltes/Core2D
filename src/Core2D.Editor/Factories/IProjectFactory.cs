// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers;

namespace Core2D.Editor.Factories
{
    /// <summary>
    /// Defines project factory contract.
    /// </summary>
    public interface IProjectFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PageContainer"/> class.
        /// </summary>
        /// <param name="project">The new template owner project.</param>
        /// <param name="name">The new template name.</param>
        /// <returns>The new instance of the <see cref="PageContainer"/>.</returns>
        PageContainer GetTemplate(ProjectContainer project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="PageContainer"/> class.
        /// </summary>
        /// <param name="project">The new page owner project.</param>
        /// <param name="name">The new page name.</param>
        /// <returns>The new instance of the <see cref="PageContainer"/>.</returns>
        PageContainer GetPage(ProjectContainer project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="DocumentContainer"/> class.
        /// </summary>
        /// <param name="project">The new document owner project.</param>
        /// <param name="name">The new document name.</param>
        /// <returns>The new instance of the <see cref="DocumentContainer"/>.</returns>
        DocumentContainer GetDocument(ProjectContainer project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="ProjectContainer"/> class.
        /// </summary>
        /// <returns>The new instance of the <see cref="ProjectContainer"/>.</returns>
        ProjectContainer GetProject();
    }
}
