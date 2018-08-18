// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Style
{
    /// <summary>
    /// Shape style extensions.
    /// </summary>
    public static class ShapeStyleExtensions
    {
        /// <summary>
        /// Clones shape style.
        /// </summary>
        /// <param name="shapeStyle">The shape style to clone.</param>
        /// <returns>The new instance of the <see cref="ShapeStyle"/> class.</returns>
        public static IShapeStyle Clone(this IShapeStyle shapeStyle)
        {
            return new ShapeStyle()
            {
                Name = shapeStyle.Name,
                Stroke = shapeStyle.Stroke.Clone(),
                Fill = shapeStyle.Fill.Clone(),
                Thickness = shapeStyle.Thickness,
                LineCap = shapeStyle.LineCap,
                Dashes = shapeStyle.Dashes,
                DashOffset = 0.0,
                LineStyle = shapeStyle.LineStyle.Clone(),
                TextStyle = shapeStyle.TextStyle.Clone(),
                StartArrowStyle = shapeStyle.StartArrowStyle.Clone(),
                EndArrowStyle = shapeStyle.EndArrowStyle.Clone()
            };
        }
    }
}
