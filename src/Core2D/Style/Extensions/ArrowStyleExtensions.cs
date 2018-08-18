// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


namespace Core2D.Style
{
    /// <summary>
    /// Arrow style extensions.
    /// </summary>
    public static class ArrowStyleExtensions
    {
        /// <summary>
        /// Clones arrow style.
        /// </summary>
        /// <returns>The new instance of the <see cref="ArrowStyle"/> class.</returns>
        public static IArrowStyle Clone(this IArrowStyle arrowStyle)
        {
            return new ArrowStyle()
            {
                Name = arrowStyle.Name,
                Stroke = arrowStyle.Stroke.Clone(),
                Fill = arrowStyle.Fill.Clone(),
                Thickness = arrowStyle.Thickness,
                LineCap = arrowStyle.LineCap,
                Dashes = arrowStyle.Dashes,
                DashOffset = arrowStyle.DashOffset,
                ArrowType = arrowStyle.ArrowType,
                IsStroked = arrowStyle.IsStroked,
                IsFilled = arrowStyle.IsFilled,
                RadiusX = arrowStyle.RadiusX,
                RadiusY = arrowStyle.RadiusY
            };
        }
    }
}
