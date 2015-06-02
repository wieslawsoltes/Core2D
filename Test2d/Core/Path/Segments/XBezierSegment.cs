// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XBezierSegment : XPathSegment
    {
        /// <summary>
        /// 
        /// </summary>
        public XPathPoint Point1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XPathPoint Point2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public XPathPoint Point3 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        /// <returns></returns>
        public static XBezierSegment Create(
            XPathPoint point1,
            XPathPoint point2,
            XPathPoint point3,
            bool isStroked,
            bool isSmoothJoin)
        {
            return new XBezierSegment()
            {
                Point1 = point1,
                Point2 = point2,
                Point3 = point3,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }
    }
}
