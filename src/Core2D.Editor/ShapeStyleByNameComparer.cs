// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Core2D.Style;

namespace Core2D.Editor
{
    /// <summary>
    /// Defines methods to support the comparison of <see cref="IShapeStyle"/> objects for equality.
    /// </summary>
    public class ShapeStyleByNameComparer : IEqualityComparer<IShapeStyle>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <see cref="IShapeStyle"/> to compare.</param>
        /// <param name="y">The second object of type <see cref="IShapeStyle"/> to compare.</param>
        /// <returns>True if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(IShapeStyle x, IShapeStyle y)
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
        /// <param name="style">The <see cref="IShapeStyle"/> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(IShapeStyle style)
        {
            if (style is null)
                return 0;
            int hashProductName = style.Name == null ? 0 : style.Name.GetHashCode();
            return hashProductName;
        }
    }
}
