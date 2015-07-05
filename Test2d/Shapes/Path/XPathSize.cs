// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XPathSize
    {
        /// <summary>
        /// 
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static XPathSize Create(double width = 0.0, double height = 0.0)
        {
            return new XPathSize()
            {
                Width = width,
                Height = height
            };
        }
    }
}
