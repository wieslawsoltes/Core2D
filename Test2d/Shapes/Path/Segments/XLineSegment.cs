// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class XLineSegment : XPathSegment
    {
        /// <summary>
        /// 
        /// </summary>
        public XPoint Point { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point"></param>
        /// <param name="isStroked"></param>
        /// <param name="isSmoothJoin"></param>
        /// <returns></returns>
        public static XLineSegment Create(
            XPoint point,
            bool isStroked,
            bool isSmoothJoin)
        {
            return new XLineSegment()
            {
                Point = point,
                IsStroked = isStroked,
                IsSmoothJoin = isSmoothJoin
            };
        }
    }
}
