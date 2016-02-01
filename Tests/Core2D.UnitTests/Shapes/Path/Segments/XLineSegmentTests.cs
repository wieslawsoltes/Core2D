// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Xunit;

namespace Core2D.UnitTests
{
    public class XLineSegmentTests
    {
        [Fact]
        [Trait("Core2D", "Path")]
        public void GetPoints_Should_Return_All_Segment_Points()
        {
            var segment = new XLineSegment()
            {
                Point = new XPoint()
            };

            var target = segment.GetPoints();

            Assert.Equal(1, target.Count());

            Assert.Contains(segment.Point, target);
        }

        [Fact]
        [Trait("Core2D", "Path")]
        public void ToString_Should_Path_Markup()
        {
            var target = new XLineSegment()
            {
                Point = new XPoint()
            };

            var actual = target.ToString();

            Assert.Equal("L0,0", actual);
        }
    }
}
