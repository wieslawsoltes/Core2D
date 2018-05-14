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
        /// Gets or sets containers.
        /// </summary>
        IList<IDockContainer> Containers { get; set; }

        /// <summary>
        /// Gets or sets views.
        /// </summary>
        IList<IDockView> Views { get; set; }

        /// <summary>
        /// Gets or sets current view.
        /// </summary>
        IDockView CurrentView { get; set; }

        /// <summary>
        /// Remove view from the container.
        /// </summary>
        /// <param name="container">The views container.</param>
        /// <param name="index">The source view index.</param>
        void RemoveView(IDockContainer container, int index);

        /// <summary>
        /// Move views in the container.
        /// </summary>
        /// <param name="container">The views container.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void MoveView(IDockContainer container, int sourceIndex, int targetIndex);

        /// <summary>
        /// Swap views in the container.
        /// </summary>
        /// <param name="container">The views container.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void SwapView(IDockContainer container, int sourceIndex, int targetIndex);

        /// <summary>
        /// Move views into another container.
        /// </summary>
        /// <param name="sourceContainer">The source views container.</param>
        /// <param name="targetContainer">The target views container.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void MoveView(IDockContainer sourceContainer, IDockContainer targetContainer, int sourceIndex, int targetIndex);

        /// <summary>
        /// Swap views into another container.
        /// </summary>
        /// <param name="sourceContainer">The source views container.</param>
        /// <param name="targetContainer">The target views container.</param>
        /// <param name="sourceIndex">The source view index.</param>
        /// <param name="targetIndex">The target view index.</param>
        void SwapView(IDockContainer sourceContainer, IDockContainer targetContainer, int sourceIndex, int targetIndex);

        /// <summary>
        /// Change current view.
        /// </summary>
        /// <param name="view">The view instance.</param>
        void OnChangeCurrentView(IDockView view);
    }
}
