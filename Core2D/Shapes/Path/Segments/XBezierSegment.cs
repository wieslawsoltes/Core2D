// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class XBezierSegment : XPathSegment
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
        public XPoint Point3 { get; set; }

        /// <summary>
        /// Creates a new <see cref="XBezierSegment"/> instance.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <param name="point3"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        /// <returns></returns>
        public static XBezierSegment Create(
            XPoint point1,
            XPoint point2,
            XPoint point3,
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
