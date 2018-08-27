// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D.Style
{
    /// <summary>
    /// Argb color extensions.
    /// </summary>
    public static class ArgbColorExtensions
    {
        /// <summary>
        /// Clones color.
        /// </summary>
        /// <param name="argbColor">The argb color to clone.</param>
        /// <returns>The new instance of the <see cref="ArgbColor"/> class.</returns>
        public static IArgbColor Clone(this IArgbColor argbColor)
        {
            return new ArgbColor()
            {
                A = argbColor.A,
                R = argbColor.R,
                G = argbColor.G,
                B = argbColor.B
            };
        }
    }
}
