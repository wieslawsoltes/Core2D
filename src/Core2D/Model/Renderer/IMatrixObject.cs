// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Renderer
{
    /// <summary>
    /// Defines transformation matrix contract.
    /// </summary>
    public interface IMatrixObject : IObservableObject
    {
        /// <summary>
        /// Gets or sets the value of the first row and first column of this matrix.
        /// </summary>
        double M11 { get; set; }

        /// <summary>
        /// Gets or sets the value of the first row and second column of this matrix.
        /// </summary>
        double M12 { get; set; }

        /// <summary>
        /// Gets or sets the value of the second row and first column of this matrix.
        /// </summary>
        double M21 { get; set; }

        /// <summary>
        /// Gets or sets the value of the second row and second column of this matrix.
        /// </summary>
        double M22 { get; set; }

        /// <summary>
        /// Gets or sets the value of the third row and first column of this matrix.
        /// </summary>
        double OffsetX { get; set; }

        /// <summary>
        /// Gets or sets the value of the third row and second column of this matrix.
        /// </summary>
        double OffsetY { get; set; }
    }
}
