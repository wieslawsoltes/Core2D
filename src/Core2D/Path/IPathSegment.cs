// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using Core2D.Shapes;

namespace Core2D.Path
{
    /// <summary>
    /// Defines path segment contract.
    /// </summary>
    public interface IPathSegment : IObservableObject, ICopyable
    {
        /// <summary>
        /// Gets or sets flag indicating whether segment is stroked.
        /// </summary>
        bool IsStroked { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether segment is smooth join.
        /// </summary>
        bool IsSmoothJoin { get; set; }

        /// <summary>
        /// Get all points in the segment.
        /// </summary>
        /// <returns>All points in the segment.</returns>
        IEnumerable<IPointShape> GetPoints();
    }
}
