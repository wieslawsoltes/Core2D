// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Style
{
    /// <summary>
    /// Line style extensions.
    /// </summary>
    public static class LineStyleExtensions
    {
        /// <summary>
        /// Clones line style.
        /// </summary>
        /// <param name="lineStyle">The line style to clone.</param>
        /// <returns>The new instance of the <see cref="LineStyle"/> class.</returns>
        public static ILineStyle Clone(this ILineStyle lineStyle)
        {
            return new LineStyle()
            {
                Name = lineStyle.Name,
                IsCurved = lineStyle.IsCurved,
                Curvature = lineStyle.Curvature,
                CurveOrientation = lineStyle.CurveOrientation,
                FixedLength = lineStyle.FixedLength.Clone()
            };
        }
    }
}
