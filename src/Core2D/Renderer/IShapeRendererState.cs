// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Core2D.Shapes;

namespace Core2D.Renderer
{
    /// <summary>
    /// Defines shape renderer state contract.
    /// </summary>
    public interface IShapeRendererState : IObservableObject
    {
        /// <summary>
        /// The X coordinate of current pan position.
        /// </summary>
        double PanX { get; set; }

        /// <summary>
        /// The Y coordinate of current pan position.
        /// </summary>
        double PanY { get; set; }

        /// <summary>
        /// The X component of current zoom value.
        /// </summary>
        double ZoomX { get; set; }

        /// <summary>
        /// The Y component of current zoom value.
        /// </summary>
        double ZoomY { get; set; }

        /// <summary>
        /// Flag indicating shape state to enable its drawing.
        /// </summary>
        IShapeState DrawShapeState { get; set; }

        /// <summary>
        /// Currently selected shape.
        /// </summary>
        IBaseShape SelectedShape { get; set; }

        /// <summary>
        /// Currently selected shapes.
        /// </summary>
        ImmutableHashSet<IBaseShape> SelectedShapes { get; set; }

        /// <summary>
        /// Image cache repository.
        /// </summary>
        IImageCache ImageCache { get; set; }
    }
}
