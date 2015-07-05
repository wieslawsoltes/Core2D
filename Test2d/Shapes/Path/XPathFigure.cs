// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XPathFigure
    {
        /// <summary>
        /// 
        /// </summary>
        public XPoint StartPoint { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<XPathSegment> Segments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsFilled { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsClosed { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="segments"></param>
        /// <param name="isFilled"></param>
        /// <param name="isClosed"></param>
        /// <returns></returns>
        public static XPathFigure Create(
            XPoint startPoint,
            IList<XPathSegment> segments,
            bool isFilled,
            bool isClosed)
        {
            return new XPathFigure()
            {
                StartPoint = startPoint,
                Segments = segments,
                IsFilled = isFilled,
                IsClosed = isClosed
            };
        }
    }
}
