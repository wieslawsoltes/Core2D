using System.Collections.Immutable;

namespace Core2D.Containers
{
    /// <summary>
    /// Defines document container interface.
    /// </summary>
    public interface IDocumentContainer : IBaseContainer
    {
        /// <summary>
        /// Gets or sets flag indicating whether document is expanded.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Gets or sets document pages.
        /// </summary>
        ImmutableArray<IPageContainer> Pages { get; set; }
    }
}
