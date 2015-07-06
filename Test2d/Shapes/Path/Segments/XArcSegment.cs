// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XArcSegment : XPathSegment
    {
        /// <summary>
        /// 
        /// </summary>
        public XPoint Point { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XPathSize Size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double RotationAngle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsLargeArc { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XSweepDirection SweepDirection { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="size"></param>
        /// <param name="rotationAngle"></param>
        /// <param name="isLargeArc"></param>
        /// <param name="sweepDirection"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        /// <returns></returns>
        public static XArcSegment Create(
            XPoint point,
            XPathSize size,
            double rotationAngle,
            bool isLargeArc,
            XSweepDirection sweepDirection,
            bool isStroked,
            bool isSmoothJoin)
        {
            return new XArcSegment()
            {
                Point = point,
                Size = size,
                RotationAngle = rotationAngle,
                IsLargeArc = isLargeArc,
                SweepDirection = sweepDirection,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }
    }
}
