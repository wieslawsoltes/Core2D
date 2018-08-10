// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Core2D.Containers.Interfaces
{
    /// <summary>
    /// Defines document container interface.
    /// </summary>
    public interface IDocumentContainer : IContainer
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
