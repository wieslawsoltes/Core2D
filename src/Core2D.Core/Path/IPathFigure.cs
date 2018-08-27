// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// Defines path figure contract.
    /// </summary>
    public interface IPathFigure : IObservableObject
    {
        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        IPointShape StartPoint { get; set; }

        /// <summary>
        /// Gets or sets segments collection.
        /// </summary>
        ImmutableArray<IPathSegment> Segments { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether path is filled.
        /// </summary>
        bool IsFilled { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether path is closed.
        /// </summary>
        bool IsClosed { get; set; }

        /// <summary>
        /// Get all points in the figure.
        /// </summary>
        /// <returns>All points in the figure.</returns>
        IEnumerable<IPointShape> GetPoints();
    }
}
