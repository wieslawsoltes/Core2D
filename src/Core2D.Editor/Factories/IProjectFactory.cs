// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Project;

namespace Core2D.Editor.Factories
{
    /// <summary>
    /// Defines project factory contract.
    /// </summary>
    public interface IProjectFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="XContainer"/> class.
        /// </summary>
        /// <param name="project">The new template owner project.</param>
        /// <param name="name">The new template name.</param>
        /// <returns>The new instance of the <see cref="XContainer"/>.</returns>
        XContainer GetTemplate(XProject project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="XContainer"/> class.
        /// </summary>
        /// <param name="project">The new page owner project.</param>
        /// <param name="name">The new page name.</param>
        /// <returns>The new instance of the <see cref="XContainer"/>.</returns>
        XContainer GetPage(XProject project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="XDocument"/> class.
        /// </summary>
        /// <param name="project">The new document owner project.</param>
        /// <param name="name">The new document name.</param>
        /// <returns>The new instance of the <see cref="XDocument"/>.</returns>
        XDocument GetDocument(XProject project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="XProject"/> class.
        /// </summary>
        /// <returns>The new instance of the <see cref="XProject"/>.</returns>
        XProject GetProject();
    }
}
