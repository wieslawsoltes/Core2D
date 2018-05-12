// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Core2D.Dock
{    
    /// <summary>
    /// Views layout contract.
    /// </summary>
    public interface IViewsLayout
    {
        /// <summary>
        /// Gets or sets panels.
        /// </summary>
        ImmutableArray<IViewsPanel> Panels { get; set; }

        /// <summary>
        /// Gets or sets current view.
        /// </summary>
        IView CurrentView { get; set; }
    }
}
