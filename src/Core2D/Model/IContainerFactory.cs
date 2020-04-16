using Core2D.Containers;

namespace Core2D
{
    /// <summary>
    /// Defines container factory contract.
    /// </summary>
    public interface IContainerFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="IPageContainer"/>.
        /// </summary>
        /// <param name="project">The new template owner project.</param>
        /// <param name="name">The new template name.</param>
        /// <returns>The new instance of the <see cref="IPageContainer"/>.</returns>
        IPageContainer GetTemplate(IProjectContainer project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="IPageContainer"/>.
        /// </summary>
        /// <param name="project">The new page owner project.</param>
        /// <param name="name">The new page name.</param>
        /// <returns>The new instance of the <see cref="IPageContainer"/>.</returns>
        IPageContainer GetPage(IProjectContainer project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="IDocumentContainer"/>.
        /// </summary>
        /// <param name="project">The new document owner project.</param>
        /// <param name="name">The new document name.</param>
        /// <returns>The new instance of the <see cref="IDocumentContainer"/>.</returns>
        IDocumentContainer GetDocument(IProjectContainer project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="IProjectContainer"/>.
        /// </summary>
        /// <returns>The new instance of the <see cref="IProjectContainer"/>.</returns>
        IProjectContainer GetProject();
    }
}
