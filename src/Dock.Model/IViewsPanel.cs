// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Dock.Model
{
    /// <summary>
    /// Views panel contract.
    /// </summary>
    public interface IViewsPanel
    {
        /// <summary>
        /// Gets or sets views.
        /// </summary>
        ImmutableArray<IView> Views { get; set; }

        /// <summary>
        /// Gets or sets current view.
        /// </summary>
        IView CurrentView { get; set; }
    }
}
