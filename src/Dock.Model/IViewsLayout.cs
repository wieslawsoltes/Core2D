// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;

namespace Dock.Model
{
    /// <summary>
    /// Views layout contract.
    /// </summary>
    public interface IViewsLayout
    {
        /// <summary>
        /// Gets or sets views.
        /// </summary>
        IList<IView> Views { get; set; }

        /// <summary>
        /// Gets or sets panels.
        /// </summary>
        IList<IViewsPanel> Panels { get; set; }

        /// <summary>
        /// Gets or sets current view.
        /// </summary>
        IView CurrentView { get; set; }

        /// <summary>
        /// Remove view from the panel.
        /// </summary>
        /// <param name="panel">The views panel.</param>
        /// <param name="index">The source view index.</param>
        void RemoveView(IViewsPanel panel, int index);

        /// <summary>
        /// Move views in the panel.
        /// </summary>
        /// <param name="panel">The views panel.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void MoveView(IViewsPanel panel, int sourceIndex, int targetIndex);

        /// <summary>
        /// Swap views in the panel.
        /// </summary>
        /// <param name="panel">The views panel.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void SwapView(IViewsPanel panel, int sourceIndex, int targetIndex);

        /// <summary>
        /// Move views into another panel.
        /// </summary>
        /// <param name="sourcePanel">The source views panel.</param>
        /// <param name="targetPanel">The target views panel.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void MoveView(IViewsPanel sourcePanel, IViewsPanel targetPanel, int sourceIndex, int targetIndex);

        /// <summary>
        /// Swap views into another panel.
        /// </summary>
        /// <param name="sourcePanel">The source views panel.</param>
        /// <param name="targetPanel">The target views panel.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void SwapView(IViewsPanel sourcePanel, IViewsPanel targetPanel, int sourceIndex, int targetIndex);

        /// <summary>
        /// Change current view.
        /// </summary>
        /// <param name="view">The view instance.</param>
        void OnChangeCurrentView(IView view);
    }
}
