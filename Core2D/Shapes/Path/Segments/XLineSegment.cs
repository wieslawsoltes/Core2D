// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Line path segment.
    /// </summary>
    public class XLineSegment : XPathSegment
    {
        /// <summary>
        /// Gets or sets end point.
        /// </summary>
        public XPoint Point { get; set; }

        /// <summary>
        /// Creates a new <see cref="XLineSegment"/> instance.
        /// </summary>
        /// <param name="point">The end point.</param>
        /// <param name="isStroked">The flag indicating whether shape is stroked.</param>
        /// <param name="isSmoothJoin">The flag indicating whether shape is smooth join.</param>
        /// <returns>The new instance of the <see cref="XLineSegment"/> class.</returns>
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
