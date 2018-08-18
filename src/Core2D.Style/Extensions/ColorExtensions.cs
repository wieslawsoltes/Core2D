// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D.Style
{
    /// <summary>
    /// Color extensions.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Clones color.
        /// </summary>
        /// <param name="color">The color to clone.</param>
        /// <returns>The new instance of the <see cref="IColor"/> class.</returns>
        public static IColor Clone(this IColor color)
        {
            switch (color)
            {
                case IArgbColor argbColor:
                    return argbColor.Clone();
                default:
                    throw new NotSupportedException($"The {color.GetType()} color type is not supported.");
            }
        }
    }
}
