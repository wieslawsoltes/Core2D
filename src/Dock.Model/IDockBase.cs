// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Dock.Model
{
    /// <summary>
    /// Dock base object contract.
    /// </summary>
    public interface IDockBase
    {
        /// <summary>
        /// Gets or sets row.
        /// </summary>
        int Row { get; set; }

        /// <summary>
        /// Gets or sets column.
        /// </summary>
        int Column { get; set; }

        /// <summary>
        /// Gets or sets views.
        /// </summary>
        IList<IDockView> Views { get; set; }

        /// <summary>
        /// Gets or sets current view.
        /// </summary>
        IDockView CurrentView { get; set; }

        /// <summary>
        /// Gets or sets children.
        /// </summary>
        IList<IDockBase> Children { get; set; }

        /// <summary>
        /// Remove view from the dock.
        /// </summary>
        /// <param name="dock">The views dock.</param>
        /// <param name="index">The source view index.</param>
        void RemoveView(IDockBase dock, int index);

        /// <summary>
        /// Move views in the dock.
        /// </summary>
        /// <param name="dock">The views dock.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void MoveView(IDockBase dock, int sourceIndex, int targetIndex);

        /// <summary>
        /// Swap views in the dock.
        /// </summary>
        /// <param name="dock">The views dock.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void SwapView(IDockBase dock, int sourceIndex, int targetIndex);

        /// <summary>
        /// Move views into another dock.
        /// </summary>
        /// <param name="sourceDock">The source views dock.</param>
        /// <param name="targetDock">The target views dock.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void MoveView(IDockBase sourceDock, IDockBase targetDock, int sourceIndex, int targetIndex);

        /// <summary>
        /// Swap views into another dock.
        /// </summary>
        /// <param name="sourceDock">The source views dock.</param>
        /// <param name="targetDock">The target views dock.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void SwapView(IDockBase sourceDock, IDockBase targetDock, int sourceIndex, int targetIndex);

        /// <summary>
        /// Change current view.
        /// </summary>
        /// <param name="view">The view instance.</param>
        void OnChangeCurrentView(IDockView view);

        /// <summary>
        /// Change current view.
        /// </summary>
        /// <param name="title">The view title.</param>
        void OnChangeCurrentView(string title);
    }
}
