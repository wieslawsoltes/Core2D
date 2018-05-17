// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Dock.Model
{
    /// <summary>
    /// Dock factory contract.
    /// </summary>
    public interface IDockFactory
    {
        /// <summary>
        /// Updates windows.
        /// </summary>
        /// <param name="windows">The target windows to update.</param>
        /// <param name="views">The source views.</param>
        /// <param name="context">The context object.</param>
        void UpdateWindows(IList<IDockWindow> windows, IList<IDockView> views, object context);

        /// <summary>
        /// Updates views.
        /// </summary>
        /// <param name="target">The target views to update.</param>
        /// <param name="views">The source views.</param>
        /// <param name="context">The context object.</param>
        void UpdateViews(IList<IDock> target, IList<IDockView> views, object context);

        /// <summary>
        /// Updates layout.
        /// </summary>
        /// <param name="layout">The target layout to update.</param>
        /// <param name="views">The source views.</param>
        /// <param name="context">The context object.</param>
        void UpdateLayout(IDockLayout layout, IList<IDockView> views, object context);

        /// <summary>
        /// Creates dock window from view.
        /// </summary>
        /// <param name="layout">The target layout.</param>
        /// <param name="context">The context object.</param>
        /// <param name="container">The views container.</param>
        /// <param name="viewIndex">The view index.</param>
        /// <param name="x">The X coordinate of window.</param>
        /// <param name="y">The Y coordinate of window.</param>
        /// <returns>The new instance of the <see cref="IDockWindow"/> class.</returns>
        IDockWindow CreateDockWindow(IDockLayout layout, object context, IDockLayout container, int viewIndex, double x, double y);

        /// <summary>
        /// Creates default layout.
        /// </summary>
        /// <param name="views">The source views.</param>
        /// <returns>The new instance of the <see cref="IDockLayout"/> class.</returns>
        IDockLayout CreateDefaultLayout(IList<IDockView> views);

        /// <summary>
        /// Creates or updates current layout.
        /// </summary>
        void CreateOrUpdateLayout();
    }
}
