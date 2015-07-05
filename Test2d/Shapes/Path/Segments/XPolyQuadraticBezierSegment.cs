// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XPolyQuadraticBezierSegment : XPathSegment
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<XPoint> Points { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="points"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        /// <returns></returns>
        public static XPolyQuadraticBezierSegment Create(
            IList<XPoint> points,
            bool isStroked,
            bool isSmoothJoin)
        {
            return new XPolyQuadraticBezierSegment()
            {
                Points = points,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }
    }
}
