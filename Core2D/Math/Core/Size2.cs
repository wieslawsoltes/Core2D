// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public struct Size2
    {
        /// <summary>
        /// 
        /// </summary>
        public double Width;

        /// <summary>
        /// 
        /// </summary>
        public double Height;

        /// <summary>
        /// Initializes a new <see cref="Size2"/> instance.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Size2(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// Creates a new <see cref="Size2"/> instance.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Size2 Create(double width, double height)
        {
            return new Size2(width, height);
        }
    }
}
