// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using Core2D.Shapes;

namespace Core2D.Containers
{
    /// <summary>
    /// Invalidate layer event arguments.
    /// </summary>
    public class InvalidateLayerEventArgs : EventArgs { }

    /// <summary>
    /// Invalidate layer event handler.
    /// </summary>
    /// <param name="sender">The sender object.</param>
    /// <param name="e">The event arguments.</param>
    public delegate void InvalidateLayerEventHandler(object sender, InvalidateLayerEventArgs e);

    /// <summary>
    /// Defines layer container interface.
    /// </summary>
    public interface ILayerContainer : IBaseContainer
    {
        /// <summary>
        /// Invalidate layer event.
        /// </summary>
        event InvalidateLayerEventHandler InvalidateLayer;

        /// <summary>
        /// Gets or sets flag indicating whether layer is visible.
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        /// Gets or sets layer shapes.
        /// </summary>
        ImmutableArray<IBaseShape> Shapes { get; set; }

        /// <summary>
        /// Invalidate layer shapes.
        /// </summary>
        void Invalidate();
    }
}
