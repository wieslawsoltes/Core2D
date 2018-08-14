// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.History;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Containers
{
    /// <summary>
    /// Defines project container interface.
    /// </summary>
    public interface IProjectContainer : IBaseContainer
    {
        /// <summary>
        /// Gets or sets project options.
        /// </summary>
        IOptions Options { get; set; }

        /// <summary>
        /// Gets or sets undo/redo history handler.
        /// </summary>
        IHistory History { get; set; }

        /// <summary>
        /// Gets or sets project style libraries.
        /// </summary>
        ImmutableArray<ILibrary<ShapeStyle>> StyleLibraries { get; set; }

        /// <summary>
        /// Gets or sets project group libraries.
        /// </summary>
        ImmutableArray<ILibrary<IGroupShape>> GroupLibraries { get; set; }

        /// <summary>
        /// Gets or sets project databases.
        /// </summary>
        ImmutableArray<Database> Databases { get; set; }

        /// <summary>
        /// Gets or sets project templates.
        /// </summary>
        ImmutableArray<IPageContainer> Templates { get; set; }

        /// <summary>
        /// Gets or sets project documents.
        /// </summary>
        ImmutableArray<IDocumentContainer> Documents { get; set; }

        /// <summary>
        /// Gets or sets project current style library.
        /// </summary>
        ILibrary<ShapeStyle> CurrentStyleLibrary { get; set; }

        /// <summary>
        /// Gets or sets project current group library.
        /// </summary>
        ILibrary<IGroupShape> CurrentGroupLibrary { get; set; }

        /// <summary>
        /// Gets or sets project current database.
        /// </summary>
        Database CurrentDatabase { get; set; }

        /// <summary>
        /// Gets or sets project current template.
        /// </summary>
        IPageContainer CurrentTemplate { get; set; }

        /// <summary>
        /// Gets or sets project current document.
        /// </summary>
        IDocumentContainer CurrentDocument { get; set; }

        /// <summary>
        /// Gets or sets project current container.
        /// </summary>
        IPageContainer CurrentContainer { get; set; }

        /// <summary>
        /// Gets or sets currently selected object.
        /// </summary>
        IObservableObject Selected { get; set; }

        /// <summary>
        /// Set current document.
        /// </summary>
        /// <param name="document">The document instance.</param>
        void SetCurrentDocument(IDocumentContainer document);

        /// <summary>
        /// Set current container.
        /// </summary>
        /// <param name="container">The container instance.</param>
        void SetCurrentContainer(IPageContainer container);

        /// <summary>
        /// Set current template.
        /// </summary>
        /// <param name="template">The template instance.</param>
        void SetCurrentTemplate(IPageContainer template);

        /// <summary>
        /// Set current database.
        /// </summary>
        /// <param name="db">The database instance.</param>
        void SetCurrentDatabase(Database db);

        /// <summary>
        /// Set current group library.
        /// </summary>
        /// <param name="library">The group library instance.</param>
        void SetCurrentGroupLibrary(ILibrary<IGroupShape> library);

        /// <summary>
        /// Set current group.
        /// </summary>
        /// <param name="library">The style library instance.</param>
        void SetCurrentStyleLibrary(ILibrary<ShapeStyle> library);

        /// <summary>
        /// Set selected value.
        /// </summary>
        /// <param name="value">The value instance.</param>
        void SetSelected(IObservableObject value);
    }
}
