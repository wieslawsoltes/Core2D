// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XQuadraticBezierSegment : XPathSegment
    {
        /// <summary>
        /// 
        /// </summary>
        public XPoint Point1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XPoint Point2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        /// <returns></returns>
        public static XQuadraticBezierSegment Create(
            XPoint point1,
            XPoint point2,
            bool isStroked,
            bool isSmoothJoin)
        {
            return new XQuadraticBezierSegment()
            {
                Point1 = point1,
                Point2 = point2,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }
    }
}
