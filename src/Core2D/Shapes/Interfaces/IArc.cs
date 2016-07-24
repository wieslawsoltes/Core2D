// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Shapes.Interfaces
{
    /// <summary>
    /// Defines arc shape contract.
    /// </summary>
    public interface IArc
    {
        /// <summary>
        /// Gets or sets top-left corner point.
        /// </summary>
        XPoint Point1 { get; set; }

        /// <summary>
        /// Gets or sets bottom-right corner point.
        /// </summary>
        XPoint Point2 { get; set; }

        /// <summary>
        /// Gets or sets point used to calculate arc start angle.
        /// </summary>
        XPoint Point3 { get; set; }

        /// <summary>
        /// Gets or sets point used to calculate arc end angle.
        /// </summary>
        XPoint Point4 { get; set; }
    }
}
