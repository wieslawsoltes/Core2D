// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Dock.Model
{
    /// <summary>
    /// Dock layout contract.
    /// </summary>
    public interface IDockLayout
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
        IList<IDockLayout> Children { get; set; }

        /// <summary>
        /// Gets or sets dock factory.
        /// </summary>
        IDockFactory Factory { get; set; }

        /// <summary>
        /// Remove view from the layout.
        /// </summary>
        /// <param name="layout">The views layout.</param>
        /// <param name="index">The source view index.</param>
        void RemoveView(IDockLayout layout, int index);

        /// <summary>
        /// Move views in the layout.
        /// </summary>
        /// <param name="layout">The views layout.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void MoveView(IDockLayout layout, int sourceIndex, int targetIndex);

        /// <summary>
        /// Swap views in the layout.
        /// </summary>
        /// <param name="layout">The views layout.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void SwapView(IDockLayout layout, int sourceIndex, int targetIndex);

        /// <summary>
        /// Move views into another layout.
        /// </summary>
        /// <param name="sourceLayout">The source views layout.</param>
        /// <param name="targetLayout">The target views layout.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void MoveView(IDockLayout sourceLayout, IDockLayout targetLayout, int sourceIndex, int targetIndex);

        /// <summary>
        /// Swap views into another layout.
        /// </summary>
        /// <param name="sourceLayout">The source views layout.</param>
        /// <param name="targetLayout">The target views layout.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void SwapView(IDockLayout sourceLayout, IDockLayout targetLayout, int sourceIndex, int targetIndex);

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
