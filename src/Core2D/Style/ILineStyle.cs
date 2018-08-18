// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Style
{
    /// <summary>
    /// Define line style contract.
    /// </summary>
    public interface ILineStyle : IObservableObject
    {
        /// <summary>
        /// Gets or sets value indicating whether line is curved.
        /// </summary>
        bool IsCurved { get; set; }

        /// <summary>
        /// Gets or sets line curvature.
        /// </summary>
        double Curvature { get; set; }

        /// <summary>
        /// Gets or sets curve orientation.
        /// </summary>
        CurveOrientation CurveOrientation { get; set; }

        /// <summary>
        /// Gets or sets line fixed length.
        /// </summary>
        ILineFixedLength FixedLength { get; set; }
    }
}
