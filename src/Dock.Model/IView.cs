// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;

namespace Dock.Model
{
    /// <summary>
    /// View contract.
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Gets view title.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets view context.
        /// </summary>
        object Context { get; }

        /// <summary>
        /// Gets or sets windows.
        /// </summary>
        ImmutableArray<IViewsWindow> Windows { get; set; }

        /// <summary>
        /// Show windows.
        /// </summary>
        void ShowWindows();

        /// <summary>
        /// Close windows.
        /// </summary>
        void CloseWindows();
    }
}
