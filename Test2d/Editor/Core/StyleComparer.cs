// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class StyleComparer : IEqualityComparer<ShapeStyle>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(ShapeStyle x, ShapeStyle y)
        {
            if (Object.ReferenceEquals(x, y))
                return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            return x.Name == y.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public int GetHashCode(ShapeStyle style)
        {
            if (Object.ReferenceEquals(style, null))
                return 0;
            int hashProductName = style.Name == null ? 0 : style.Name.GetHashCode();
            return hashProductName;
        }
    }
}
