// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
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
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Size2(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <summary>
        /// 
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
