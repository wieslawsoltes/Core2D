// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Shapes.Interfaces
{
    /// <summary>
    /// Defines line shape contract.
    /// </summary>
    public interface ILine
    {
        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        PointShape Start { get; set; }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        PointShape End { get; set; }
    }
}
