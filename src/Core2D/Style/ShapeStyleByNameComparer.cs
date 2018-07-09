// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Core2D.Style
{
    /// <summary>
    /// Defines methods to support the comparison of <see cref="ShapeStyle"/> objects for equality.
    /// </summary>
    public class ShapeStyleByNameComparer : IEqualityComparer<ShapeStyle>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="ShapeStyle"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="ShapeStyle"/> to compare.</param>
        /// <returns>True if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(ShapeStyle x, ShapeStyle y)
        {
            if (object.ReferenceEquals(x, y))
                return true;

            if (x is null || y is null)
                return false;

            return x.Name == y.Name;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="style">The <see cref="ShapeStyle"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(ShapeStyle style)
        {
            if (style is null)
                return 0;
            int hashProductName = style.Name == null ? 0 : style.Name.GetHashCode();
            return hashProductName;
        }
    }
}
