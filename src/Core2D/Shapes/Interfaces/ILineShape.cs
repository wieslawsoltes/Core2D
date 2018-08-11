// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Shapes.Interfaces
{
    /// <summary>
    /// Defines line shape contract.
    /// </summary>
    public interface ILineShape : IBaseShape
    {
        /// <summary>
        /// Gets or sets start point.
        /// </summary>
        IPointShape Start { get; set; }

        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        IPointShape End { get; set; }
    }
}
