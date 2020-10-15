using Core2D.Containers;

namespace Core2D
{
    /// <summary>
    /// Defines container factory contract.
    /// </summary>
    public interface IContainerFactory
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PageContainer"/>.
        /// </summary>
        /// <param name="project">The new template owner project.</param>
        /// <param name="name">The new template name.</param>
        /// <returns>The new instance of the <see cref="PageContainer"/>.</returns>
        PageContainer GetTemplate(ProjectContainer project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="PageContainer"/>.
        /// </summary>
        /// <param name="project">The new page owner project.</param>
        /// <param name="name">The new page name.</param>
        /// <returns>The new instance of the <see cref="PageContainer"/>.</returns>
        PageContainer GetPage(ProjectContainer project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="DocumentContainer"/>.
        /// </summary>
        /// <param name="project">The new document owner project.</param>
        /// <param name="name">The new document name.</param>
        /// <returns>The new instance of the <see cref="DocumentContainer"/>.</returns>
        DocumentContainer GetDocument(ProjectContainer project, string name);

        /// <summary>
        /// Creates a new instance of the <see cref="ProjectContainer"/>.
        /// </summary>
        /// <returns>The new instance of the <see cref="ProjectContainer"/>.</returns>
        ProjectContainer GetProject();
    }
}
