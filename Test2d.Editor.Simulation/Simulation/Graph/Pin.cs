// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Test2d;

namespace TestSIM
{
    /// <summary>
    /// 
    /// </summary>
    public class Pin
    {
        /// <summary>
        /// 
        /// </summary>
        public XPoint Point { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsInverted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="isInverted"></param>
        /// <returns></returns>
        public static Pin Create(XPoint point, bool isInverted)
        {
            return new Pin()
            {
                Point = point,
                IsInverted = isInverted
            };
        }
    }
}
