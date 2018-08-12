// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Data;

namespace Core2D.Shapes
{
    /// <summary>
    /// Defines base shape contract.
    /// </summary>
    public interface IBaseShape : IObservableObject, IDrawable, ISelectable, ICopyable
    {        
        /// <summary>
        /// Gets or sets shape owner.
        /// </summary>
        IBaseShape Owner { get; set; }

        /// <summary>
        /// Indicates shape state options.
        /// </summary>
        ShapeState State { get; set; }

        /// <summary>
        /// Gets or sets shape <see cref="Context"/>.
        /// </summary>
        Context Data { get; set; }

        /// <summary>
        /// Get all points in the shape.
        /// </summary>
        /// <returns>All points in the shape.</returns>
        IEnumerable<IPointShape> GetPoints();
    }
}
