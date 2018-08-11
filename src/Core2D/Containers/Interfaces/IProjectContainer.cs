// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Data;
using Core2D.History;
using Core2D.Shapes.Interfaces;
using Core2D.Style;

namespace Core2D.Containers.Interfaces
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
        ImmutableArray<Library<ShapeStyle>> StyleLibraries { get; set; }

        /// <summary>
        /// Gets or sets project group libraries.
        /// </summary>
        ImmutableArray<Library<IGroupShape>> GroupLibraries { get; set; }

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
        Library<ShapeStyle> CurrentStyleLibrary { get; set; }

        /// <summary>
        /// Gets or sets project current group library.
        /// </summary>
        Library<IGroupShape> CurrentGroupLibrary { get; set; }

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
        SelectableObject Selected { get; set; }
    }
}
